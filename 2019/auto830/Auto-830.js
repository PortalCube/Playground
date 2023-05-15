// Auto 830
// Version 1.5.2

var version = "1.5.2";
var menu_version = "1.5.1";
var modal_version = "1.5.1";
var setting_version = "1";

var Band = {};
var option;

var intervalID;
var process = [];

var scriptTime = 0;
var latestAlert = 0;

var time_regex = [
    /^지금막$/g, // 60초 미만
    /^(\d{1,2})분 전$/g, // 1분 ~ 59분
    /^(\d{1})시간 전$/g, // 1시간 ~ 9시간
    /^(\d{1})시간 전$/g, // 10시간 ~ 23시간
    /^(\d{4})년 (\d{1,2})월 (\d{1,2})일 (오후|오전) (\d{1,2}):(\d{1,2})$/g // 24시간 ~
];

function WaitForReady() {
    if (!document.querySelector(".uLoading") && !!document.querySelector(".postWrap")) {
        setTimeout(initialize, 1000);
    } else {
        setTimeout(WaitForReady, 100);
    }
}

function initialize() {
    LoadSetting();

    InnerLog(moment().format());
    Log("Auto 830을 시작합니다. 버전: " + version);

    // Moment.js Customize
    moment.locale('ko', {
        relativeTime: {
            future: "%s 후에",
            past: "곧",
            s: '%d초',
            ss: '%d초',
            m: "%d분",
            mm: "%d분",
            h: "%d시간",
            hh: "%d시간",
            d: "%d일",
            dd: "%d일",
            M: "%d개월",
            MM: "%d개월",
            y: "%d년",
            yy: "%d년"
        }
    });

    InsertMenu();

    InsertModal();

    // 밴드 만들기
    CreateBand();
    
    // 댓글에 추출 함수 삽입
    openComment();

    // 불러온 HTML에 자바 스크립트 삽입
    htmlAttatch();
}

function InsertMenu() {
    Log("Menu HTML을 삽입합니다.");
    var div = document.querySelector("aside#banner .theiaStickySidebar").appendChild(document.createElement("div"));
    var html = loadSync("https://raw.githubusercontent.com/PortalCube/Auto830/master/document/menu_" + menu_version + ".html");

    if (html != null) {
        Log("정상적으로 Menu HTML을 불러왔습니다.");
        div.innerHTML = html;
    } else {
        var answer = prompt("Error: Github에서 HTML을 불러오는데 실패했습니다.\n수동으로 계속하려면 Menu HTML 코드를 입력해주세요.");
        if (answer != null || answer != "") { return; }
        div.innerHTML = answer;
    }
}

function InsertModal() {
    Log("설정 Modal HTML을 삽입합니다.");
    var modal = document.querySelector("body").appendChild(document.createElement("div"));
    var html = loadSync("https://raw.githubusercontent.com/PortalCube/Auto830/master/document/modal_" + modal_version + ".html");

    if (html != null) {
        Log("정상적으로 설정 Modal HTML을 불러왔습니다.");
        modal.innerHTML = html;
    } else {
        var answer = prompt("Error: Github에서 HTML을 불러오는데 실패했습니다.\n 수동으로 계속하려면 설정 Modal HTML 코드를 입력해주세요.");
        if (answer != null || answer != "") { return; }
        modal.innerHTML = answer;
    }
}

function htmlAttatch() {
    var modal = document.getElementById("auto830-setting-modal");

    var start_btn =  document.getElementById("auto830_process_btn");
    var option_btn = document.getElementById("auto830_option_btn");
    var comment_btn = document.getElementById("auto830_comment_btn");
    var save_btn = document.getElementById("auto830_setting_save_btn");
    var close_btn = document.getElementById("auto830_setting_close_btn");
    var reset_btn = document.getElementById("auto830_setting_reset_btn");

    start_btn.onclick = function () {
        if (intervalID === undefined) { StartProcess(); } else { StopProcess(); }
    };

    option_btn.onclick = function () {
        if (intervalID !== undefined) { alert("프로세스가 가동되는 동안에는 설정할 수 없습니다."); return; }
        RefreshModal();
        modal.style.display = "block";
    };

    comment_btn.onclick = function () {
        attachFunction();
    };

    save_btn.onclick = function () {
        if (!SaveSetting()) { return; }
        LoadSetting();
        modal.style.display = "none";
    };

    close_btn.onclick = function () {
        if (!confirm("경고: 저장하지 않은 모든 변경 사항이 무시됩니다. 계속할까요?")) { return; }
        modal.style.display = "none";
    };

    reset_btn.onclick = function () {
        if (!confirm("경고: 모든 설정을 초기화합니다. 계속할까요?")) { return; }
        ResetSetting();
        RefreshModal();
    };

    $(".auto830 .version").text("Auto 830 - " + version);
}

// =======================
// Band Manipulation Code
// =======================

function openComment() {
    // 각 댓글창을 열기 -- array index에 따라서 각 댓글에 접근해야 하기에 필수적인 작업
    $("._commentMainBtn").each(function (index, element) {
        if (index >= option.process.limitPost) { return; }
        element.click();
    });
    Log("모든 포스트의 댓글을 열었습니다!");

    // 약간의 딜레이 후에 모든 이미지 업로드 버튼의 onChange 이벤트에 코드 삽입
    ReattachFunction();
}

function attachFunction() {
    $(".inputUploadFile._imageUploadButton").each(function (index, element) {
        $(element).change(function () {
            Band.files = this.files;
            previewImage();
        });
    });
    Log("댓글 파일 업로드에 추출 함수를 삽입했습니다!");
}

function ReattachFunction() {
    setTimeout(attachFunction, 1000);
}

function previewImage() {
    FileAPI.readAsDataURL(Band.files[0], function (evt) {
        if (evt.type == 'load') {
            // Success!
            $("#auto830_img").attr("src", evt.result);
            ReattachFunction();
            Log("이미지를 성공적으로 추출했어요!");
        } else if (evt.type == 'progress') {
            // Processing
            //console.log(evt.loaded/evt.total);
        } else {
            // Error
            Log("이미지를 추출하는데 실패했어요...", 2);
        }
    });
}

function CreateBand() {
    // 메인 객체인 band 초기화

    // reload function
    $('#lnb>ul>li>a').each(function (index, element) {
        if ($(element).text().indexOf("전체글") != -1) {
            Band.reload = function () {
                element.click();
            }
        }
    });

    // refine function
    Band.refine = function (div, i) {
        var article = {};
        if (div.innerHTML == "") {
            Log("게시글의 내용이 비어있습니다. 내용이 너무 길어서 스킵되었거나, Band 서비스를 제대로 불러오지 못했을 수 있습니다.", 1);
            return undefined;
        }
        article.id = i;
        article.author = div.querySelector(".postWriterInfoWrap > span.ellipsis > strong.text").textContent.trim();
        if (!!div.querySelector(".comment._commentCountBtn > span")) {
            article.comment = div.querySelector(".comment._commentCountBtn > span").textContent.trim();
        } else {
            article.comment = "0";
        }
        article.time = div.querySelector("time").textContent.trim();
        if (div.querySelector(".txtBody") == null) {
            Log("게시글에 텍스트가 존재하지 않습니다. 830 게시글이 아닐 것으로 판단, 게시글을 넘깁니다.", 1);
            return undefined;
        }
        article.textContent = div.querySelector(".txtBody").textContent.trim();
        article.element = div;
        return article;
    };

    // sendComment function
    Band.sendComment = function (id) {
        if (Band.files === undefined) { return; }

        var post = Band.post[id].element;
        var obj = post.querySelector(".inputUploadFile._imageUploadButton");
        if (!obj) {
            post.scrollIntoView();
            AddTask(function () { Band.sendComment(id); }, 100, "댓글을 전송합니다.");
            return;
        }
        obj.files = Band.files;
        fireEvent(obj, "change");
        checkDisabled(post);
    };

    Log("Band 객체를 성공적으로 생성했어요!");
}

function bandPostUpdate() {
    // set post
    var divs = $('.postWrap>div>div.cCard.gBoxShadow');
    Band.post = [];
    divs.each(function (index, element) {
        if (index >= option.process.limitPost) { return; }
        var post = Band.refine(element, index);
        if (post !== undefined) {
            Band.post.push(post);
        }
    });

    // set recent
    Band.recent = Band.post[0];
}

function checkDisabled(post) {
    var btn = post.querySelector("._sendMessageButton");

    if (!btn) {
        post.scrollIntoView();
        AddTask(function () { checkDisabled(post); }, 100, "댓글을 전송합니다.");
        return;
    }

    if (!btn.classList.contains("-disabled")) {
        btn.click();
        Log("댓글을 전송했습니다!");
        if (option.IFTTT.sendAlert) {
            SendMessage(moment().format("H:mm:ss") + "에 댓글을 달았습니다. Auto 830의 가동이 중지됩니다.");
        }
    } else {
        recheckDisabled(post);
    }
}

function recheckDisabled(post) {
    setTimeout(function () { checkDisabled(post); }, 500);
}

function checkPost() {
    for (var i in Band.post) {
        var element = Band.post[i];
        var nameCheck = element.textContent.indexOf("오늘의 830") != -1;
        var timeCheck = false;

        for (var x = 0; x < 3; x++) {
            timeCheck = timeCheck || time_regex[x].test(element.time);
        }

        if (nameCheck && timeCheck) {
            Log("830 게시글을 찾았어요!");
            openComment();
            ReserveComment(element);
            return;
        }
    }

    AddReloadTask();
}

function ReserveComment(post) {

    var time = 0;

    if (option.commentDelay.active) {

        var min_rand = option.commentDelay.minvalue;
        var max_rand = option.commentDelay.maxvalue;

        if (max_rand < min_rand) {
            // Swap
            var t = max_rand;
            max_rand = min_rand;
            min_rand = t;
        }

        var time = Math.floor(Math.random() * (max_rand - min_rand)) + min_rand;
    }

    if (option.IFTTT.postAlert) {
        SendMessage("게시글이 올라왔어요!\n" + moment([1970, 1, 1]).add(time, 'ms').format("H시간 mm분 ss초") + " 뒤에 댓글을 전송할 예정입니다.\n내용: " + post.textContent);
    }

    post.element.scrollIntoView();

    AddTask(function () { Band.sendComment(post.id); }, time, "댓글을 전송합니다.");
}

function setStatusText(str) {
    $("#auto830_status").html(str.replace("\n", "<br>"));
}

// =======================
//      Some Utility
// =======================

function Log(str, mode = 0) {
    str = "[Auto-830]" + str;
    switch (mode) {
        case 1:
            console.warn(str);
            break;
        case 2:
            console.error(str);
            break;
        default:
            console.log(str);
            break;
    }

    InnerLog(str);
}

function InnerLog(str) {
    if (option === undefined || !option.log.active) {return;}
    str = "[" + moment().format("H:mm:ss") + "]" + str;
    var log = !localStorage.getItem("auto830_log") ? "" : localStorage.getItem("auto830_log");
    var str = log + "\n" + str;
    localStorage.setItem("auto830_log", str);
}

function SendMessage(msg) {
    if (!option.IFTTT.active) { return; }
    $.post("https://maker.ifttt.com/trigger/830_notification/with/key/" + option.IFTTT.key, { value1: msg });
    Log("스마트폰에 메세지를 전송했습니다. > \"" + msg + "\"")
}

function fireEvent(element, e) {
    var event = new Event(e, { bubbles: true });
    element.dispatchEvent(event);
}

function loadSync(strUrl) {
    var strReturn = null;
    $.ajax({
      url: strUrl,
      success: function(html) {
        strReturn = html;
      },
      async:false
    });
  
    return strReturn;
}

// =======================
//   Process System Code
// =======================

function Tick() {
    if (process.length == 0) {
        Log("프로세스가 없어요! 가동을 중지합니다.");
        StopProcess();
        return 0;
    }

    process[0].time = process[0].time - option.process.tickRate;
    scriptTime += option.process.tickRate;

    if (option.IFTTT.timeAlert && moment([1970, 1, 1]).add(scriptTime, 'ms').hours() > latestAlert) {
        SendMessage("현재 " + ++latestAlert + "시간째 대기 중이에요!");
    }

    setStatusText(moment.duration(process[0].time).humanize(true) + " " + process[0].text);

    if (process[0].time <= 0) {
        InnerLog(process[0].text);
        process[0].func();
        process.shift();
    }
}

function StartProcess() {

    if (Band.files === undefined) { alert("이미지가 선택되지 않았습니다. 이미지를 선택하고 실행해주세요."); return; }

    Log("지금부터 가동합니다!");

    document.getElementById("auto830-setting-modal").style.display = "none";

    if (option.waiting.active) {
        AddStartTask();
    } else {
        AddReloadTask();
    }

    // 메인 인터벌 세팅 → 가동
    intervalID = setInterval(Tick, option.process.tickRate);

    // 버튼 업데이트
    $("#auto830_process_btn").addClass("deactivate");
    $("#auto830_process_btn").removeClass("active");
    $("#auto830_option_btn").addClass("inactivate");
    $("#auto830_option_btn").removeClass("option");
    $("#auto830_process_btn").text("Auto 830 중지하기");
}

function StopProcess() {
    // 메인 인터벌 가동 중지
    window.clearInterval(intervalID);

    // 변수 초기화
    intervalID = undefined;
    process = [];
    scriptTime = 0;
    latestAlert = 0;

    // 버튼 업데이트
    $("#auto830_process_btn").addClass("active");
    $("#auto830_process_btn").removeClass("deactivate");
    $("#auto830_option_btn").addClass("option");
    $("#auto830_option_btn").removeClass("inactivate");
    $("#auto830_process_btn").text("Auto 830 가동하기");

    setStatusText("Auto 830을 가동하려면\n아래의 버튼을 눌러주세요.");
    Log("가동이 중지되었어요!");
}

function AddTask(func, time, text, order = false) {
    if (!order) {
        process.push({ func: func, time: time, text: text });
    } else {
        process.unshift({ func: func, time: time, text: text });
    }
}

function AddReloadTask() {
    AddTask(Band.reload, option.process.refreshTime, "밴드 게시글을 새로고침합니다.");
    AddTask(bandPostUpdate, option.process.postUpdateTIme, "밴드 게시글 목록을 불러옵니다.");
    //AddTask(openComment, 3000, "댓글을 불러옵니다.");
    AddTask(checkPost, option.process.postCheckTime, "찾는 게시글이 있는지 확인합니다.");
}

function AddStartTask() {
    var time = moment();
    var h = option.waiting.time.split(":")[0];
    var m = option.waiting.time.split(":")[1];
    var w_time = moment().hour(h).minute(m).startOf('minute');

    if (w_time.isBefore(time)) {
        w_time = w_time.add(1, 'd');
    }

    if (option.IFTTT.startAlert) {
        AddTask(function () { SendMessage(w_time.format("H:mm") + " 이 되었어요! 가동을 시작합니다.") }, w_time.diff(moment()), "가동을 시작합니다.");
        AddTask(AddReloadTask, 0, "가동을 시작합니다.");
    } else {
        AddTask(AddReloadTask, w_time.diff(moment()), "가동을 시작합니다.");
    }
}


// =======================
//   Auto 830 Setting
// =======================

function LoadSetting() {
    var opt = JSON.parse(localStorage.getItem("auto830_setting"));
    if (opt === null || opt.version != setting_version ) {
        Log("사용자의 브라우저에 설정이 없거나 설정의 버전이 달라요! 설정을 초기화 합니다.", 1)
        ResetSetting();
    }
    option = JSON.parse(localStorage.getItem("auto830_setting"));
}

function ResetSetting() {
    localStorage.setItem("auto830_setting", '{"version":"1","waiting":{"active":true,"time":"07:00"},"commentDelay":{"active":true,"minvalue":60000,"maxvalue":600000},"IFTTT":{"active":false,"key":"","timeAlert":true,"startAlert":true,"postAlert":true,"sendAlert":true},"process":{"tickRate":100,"refreshTime":285000,"postUpdateTIme":12000,"postCheckTime":3000,"limitPost":5},"log":{"active":true}}');
}

function RefreshModal() {

    LoadSetting();

    var modal = document.querySelector("#auto830-setting-modal");
    var section;
    // -------- 대기 기능 ---------

    section = modal.querySelector("#waiting");
    getElement(section, "#onoff").checked = option.waiting.active;
    getElement(section, "#time").value = option.waiting.time;

    // -------- 댓글 작성 딜레이 ---------

    section = modal.querySelector("#commentDelay");
    getElement(section, "#onoff").checked = option.commentDelay.active;
    getElement(section, "#minvalue").value = option.commentDelay.minvalue;
    getElement(section, "#maxvalue").value = option.commentDelay.maxvalue;

    // -------- IFTTT 앱 알림 서비스 ---------

    section = modal.querySelector("#IFTTT");
    getElement(section, "#onoff").checked = option.IFTTT.active;
    getElement(section, "#apikey").value = option.IFTTT.key;
    getElement(section, "#timeAlert").checked = option.IFTTT.timeAlert;
    getElement(section, "#startAlert").checked = option.IFTTT.startAlert;
    getElement(section, "#postAlert").checked = option.IFTTT.postAlert;
    getElement(section, "#sendAlert").checked = option.IFTTT.sendAlert;
    //getElement(section, "#timestamp").checked = option.IFTTT.timestamp;

    // -------- 830 프로세스 ---------

    section = modal.querySelector("#process");

    getElement(section, "#tickRate").value = option.process.tickRate;
    getElement(section, "#refreshTime").value = option.process.refreshTime;
    getElement(section, "#postUpdateTIme").value = option.process.postUpdateTIme;
    getElement(section, "#postCheckTime").value = option.process.postCheckTime;
    getElement(section, "#limitPost").value = option.process.limitPost;

    // -------- Auto 830 로그 ---------
    section = modal.querySelector("#log");
    getElement(section, "#onoff").checked = option.log.active;
}

function SaveSetting() {
    
    var new_option = JSON.parse(localStorage.getItem("auto830_setting"));
    var modal = document.querySelector("#auto830-setting-modal");
    var section;

    new_option.version = setting_version;

    // -------- 대기 기능 ---------

    section = modal.querySelector("#waiting");
    //new_option.waiting = {};
    new_option.waiting.active = getElement(section, "#onoff").checked;

    if (new_option.waiting.active && !InputCheck(section, "#time")) {
        alert("저장 실패: 대기 기능의 시각이 올바르지 않습니다.");
        return false;
    } else {
        new_option.waiting.time = getElement(section, "#time").value;
    }

    // -------- 댓글 작성 딜레이 ---------

    section = modal.querySelector("#commentDelay");
    //new_option.commentDelay = {};
    new_option.commentDelay.active = getElement(section, "#onoff").checked;


    if (new_option.commentDelay.active) {

        if (!InputCheck(section, "#minvalue")) {
            alert("저장 실패: 댓글 작성 딜레이의 최소값이 올바르지 않습니다.\n값은 0 이상의 정수이어야합니다.");
            return false;
        }

        if (!InputCheck(section, "#maxvalue")) {
            alert("저장 실패: 댓글 작성 딜레이의 최대값이 올바르지 않습니다.\n값은 0 이상의 정수이어야합니다.");
            return false;
        }

        if (Number(getElement(section, "#minvalue").value) > Number(getElement(section, "#maxvalue").value)) {
            alert("저장 실패: 댓글 작성 딜레이의 최소값이 최대값보다 큽니다.");
            return false;
        }

        new_option.commentDelay.minvalue = Number(getElement(section, "#minvalue").value);
        new_option.commentDelay.maxvalue = Number(getElement(section, "#maxvalue").value);
    }

    // -------- IFTTT 앱 알림 서비스 ---------

    section = modal.querySelector("#IFTTT");
    //new_option.IFTTT = {};
    new_option.IFTTT.active = getElement(section, "#onoff").checked;

    if (new_option.IFTTT.active) {
        new_option.IFTTT.key = getElement(section, "#apikey").value;
        new_option.IFTTT.timeAlert = getElement(section, "#timeAlert").checked;
        new_option.IFTTT.startAlert = getElement(section, "#startAlert").checked;
        new_option.IFTTT.postAlert = getElement(section, "#postAlert").checked;
        new_option.IFTTT.sendAlert = getElement(section, "#sendAlert").checked;
        //new_option.IFTTT.timestamp = getElement(section, "#timestamp").checked;
    }

    // -------- 830 프로세스 ---------

    section = modal.querySelector("#process");
    //new_option.process = {};

    if (!InputCheck(section, "#tickRate")) {
        alert("저장 실패: 830 프로세스의 Tick Rate 값이 올바르지 않습니다.\n값은 50에서 5000사이의 정수이어야 합니다.");
        return false;
    }

    if (!InputCheck(section, "#refreshTime")) {
        alert("저장 실패: 830 프로세스의 게시글 새로고침 시간값이 올바르지 않습니다.\n값은 0 이상의 정수이어야 합니다.");
        return false;
    }

    if (!InputCheck(section, "#postUpdateTIme")) {
        alert("저장 실패: 830 프로세스의 밴드 게시글 불러오기 시간값이 올바르지 않습니다.\n값은 0 이상의 정수이어야 합니다.");
        return false;
    }

    if (!InputCheck(section, "#postCheckTime")) {
        alert("저장 실패: 830 프로세스의 밴드 게시글 확인 시간값이 올바르지 않습니다.\n값은 0 이상의 정수이어야 합니다.");
        return false;
    }

    if (!InputCheck(section, "#limitPost")) {
        alert("저장 실패: 830 프로세스의 밴드 게시글 인식 갯수 제한값이 올바르지 않습니다.\n값은 1 이상의 정수이어야 합니다.");
        return false;
    }

    new_option.process.tickRate = Number(getElement(section, "#tickRate").value);
    new_option.process.refreshTime = Number(getElement(section, "#refreshTime").value);
    new_option.process.postUpdateTIme = Number(getElement(section, "#postUpdateTIme").value);
    new_option.process.postCheckTime = Number(getElement(section, "#postCheckTime").value);
    new_option.process.limitPost = Number(getElement(section, "#limitPost").value);

    // -------- Auto 830 로그 ---------
    section = modal.querySelector("#log");
    //new_option.log = {};
    new_option.log.active = getElement(section, "#onoff").checked;
    localStorage.setItem("auto830_setting", JSON.stringify(new_option));
    return true;
}

function InputCheck(section, element) {
    return getElement(section, element).validity.valid && getElement(section, element).value != "";
}

function getElement(section, element) {
    return section.querySelector(element);
}

WaitForReady();