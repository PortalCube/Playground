if (!String.format) {
    String.format = function (format) {
        var args = Array.prototype.slice.call(arguments, 1);
        return format.replace(/{(\d+)}/g, function (match, number) {
            return typeof args[number] != 'undefined'
                ? args[number]
                : match
                ;
        });
    };
}

var jobList;
var skillList = {};
var selectedJob;
var selectedSkill = [];
var selectedCoreSkill = [];
var coreList = [];

var coreCombinationResult = [];
var goalLength;
var goal = {};

var jobCategoryName = {
    adventurer: "모험가",
    cygnus: "시그너스 기사단",
    resistance: "레지스탕스",
    hero: "영웅",
    nova: "노바",
    lef: "레프",
}

function ReplaceErrorImage(image) {
    image.onerror = "";
    image.src = "image/no_image.png";
    return true;
}

function LoadJob() {
    var html = `
    <div class="card m-2">
        <img src="image/vmatrix/job_image/{0}" class="card-img-top" alt="{1}" onerror="ReplaceErrorImage(this);">
        <div class="card-body">
            <h5 class="card-title">{1}</h5>
            <button class="btn btn-primary" onclick="SetJob({2});">선택</button>
        </div>
    </div>
    `;


    var disabledHTML = `
    <div class="card m-2">
        <img src="image/vmatrix/job_image/{0}" class="card-img-top" alt="{1}" onerror="ReplaceErrorImage(this);">
        <div class="card-body">
            <h5 class="card-title">{1}</h5>
            <div class="d-inline-block" tabindex="0" data-toggle="tooltip" data-html="true" data-placement="right" title="<small>이 직업은 사용할 수 없습니다..<small>">
                <button class="btn btn-secondary" disabled>선택</button>
            </div>
        </div>
    </div>
    `;

    $.getJSON("job.json", function (json) {
        jobList = json;
        for (i in json) {
            var selectedHTML = html;
            if (typeof json[i].skill === "undefined") {
                selectedHTML = disabledHTML;
            }
            $("#job-" + json[i].category + " > div").append(String.format(selectedHTML, json[i].image, json[i].name, i));
        }
    });

    // Activate Tooltip
    $('body').tooltip({
        selector: '[data-toggle=tooltip]'
    });
}

function SetJob(num) {
    if (typeof selectedJob === "undefined" || selectedJob != jobList[num]) {
        selectedJob = jobList[num];

        LoadSkill();

        $('#currentJob > img').fadeOut(200, function () {
            $(this).attr("src", String.format("image/vmatrix/job_image/{0}", selectedJob.image)).fadeIn(200);
        });

        var categoryName;

        if (selectedJob.category == "etc") {
            categoryName = selectedJob.name;
        } else {
            categoryName = jobCategoryName[selectedJob.category];
        }

        $('#currentJob span').fadeOut(200, function () {
            $(this).text(categoryName).fadeIn(200);
        });

        $('#currentJob strong').fadeOut(200, function () {
            $(this).text(selectedJob.name).fadeIn(200);
        });
    }
    setTimeout(function () { $('#select-skill-tab').tab('show'); }, 300);
}

function LoadSkill() {
    var skill_html = `
    <button type="button" id="skill_btn_{0}" class="btn btn-outline-dark btn-skill border-0" onmouseover="SkillPreview({0}, true);" onclick="AddSkill({0});">
        <img src="image/vmatrix/skill/skill_{0}_icon.png">
    </button>
    `;

    var core_html = `
    <button type="button" id="core_btn_{0}" class="btn btn-outline-dark btn-skill border-0" onmouseover="SkillPreview({0}, false);" onclick="AddSkillToCore({0});">
        <img src="image/vmatrix/skill/skill_{0}_icon.png">
    </button>
    `;

    skillList = {};

    var skills = selectedJob.skill;

    var image = document.querySelectorAll('.skill-description');

    for (x in image) {
        image[x].src = String.format("image/vmatrix/skill/skill_{0}.png", skills[0].id);
    }

    // 스킬 초기화
    document.querySelector("#skillSelectCard .d-flex").innerHTML = "";
    document.querySelector("#coreSkillList").innerHTML = "";

    ResetSkill();
    ResetSkillFromCore();
    ResetCore();

    for (x in skills) {
        skillList[skills[x].id] = skills[x].name;
        console.log(skillList[skills[x].id]);
        $('#skillSelectCard .d-flex').append(String.format(skill_html, skills[x].id));
        $('#coreSkillList').append(String.format(core_html, skills[x].id));
    }

}

function SkillPreview(id, isSkill) {
    if (isSkill) {
        document.querySelector("#skillDescriptionCard > img").src = String.format("image/vmatrix/skill/skill_{0}.png", id);
    } else {
        document.querySelector("#coreSkillSelectCard > img").src = String.format("image/vmatrix/skill/skill_{0}.png", id);
    }

}

function AddSkill(id) {
    var html = `
    <button type="button" id="selected_skill_btn_{0}" class="btn btn-outline-dark btn-skill border-0" onmouseover="SkillPreview({0}, true);" onclick="RemoveSkill({0});">
        <img src="image/vmatrix/skill/skill_{0}_icon.png">
    </button>
    `;

    $('#skill_btn_' + id).attr("disabled", true);
    $('#selectedSkillCard .d-flex').append(String.format(html, id));

    selectedSkill.push(id);
    SkillNextButtonCheck();
}

function RemoveSkill(id) {
    $('#skill_btn_' + id).attr("disabled", false);
    $('#selected_skill_btn_' + id).remove();

    // selectedSkill 에서 "특정" 요소 지우기..
    var index = selectedSkill.indexOf(id);
    if (index > -1) { selectedSkill.splice(index, 1); }
    SkillNextButtonCheck();
}

function ResetSkill() {
    // RemoveSkill은 Array의 크기를 변화시키므로 For문을 끝에서부터 돌림
    for (var x = selectedSkill.length; x > -1; x--) {
        RemoveSkill(selectedSkill[x]);
    }
    SkillNextButtonCheck();
}

function SkillNextButtonCheck() {
    if (selectedSkill.length > 0) { $('#skill-next-button').attr("disabled", false); }
    else { $('#skill-next-button').attr("disabled", true); }
}

function AddSkillToCore(id) {

    if (selectedCoreSkill.length >= 3) { return; }

    var html = `
    <button type="button" id="selected_core_btn_{0}" class="btn btn-outline-dark btn-skill border-0" onmouseover="SkillPreview({0}, false);" onclick="RemoveSkillFromCore({0});">
        <img src="image/vmatrix/skill/skill_{0}_icon.png">
    </button>
    `;

    $('#core_btn_' + id).attr("disabled", true);
    $('#coreSelectedSkill').append(String.format(html, id));

    selectedCoreSkill.push(id);
    CoreAddButtonCheck();
}

function RemoveSkillFromCore(id) {
    $('#core_btn_' + id).attr("disabled", false);
    $('#selected_core_btn_' + id).remove();

    // selectedCoreSkill 에서 "특정" 요소 지우기..
    var index = selectedCoreSkill.indexOf(id);
    if (index > -1) { selectedCoreSkill.splice(index, 1); }
    CoreAddButtonCheck();
}

function ResetSkillFromCore() {
    // RemoveSkillFromCore는 Array의 크기를 변화시키므로 For문을 끝에서부터 돌림
    for (var x = selectedCoreSkill.length; x > -1; x--) {
        RemoveSkillFromCore(selectedCoreSkill[x]);
    }
    CoreAddButtonCheck();
}

function CoreAddButtonCheck() {
    if (selectedCoreSkill.length >= 3) { $('#core-add-button').attr("disabled", false); }
    else { $('#core-add-button').attr("disabled", true); }
}

function AddCore() {
    if (selectedCoreSkill.length < 3) {return;}
    
    var html = `
    <div id="core_{3}_panel" class="row border rounded m-1 p-2" style="display: none;">
        <div class="col-sm-7">
            <div class="d-flex justify-content-center flex-wrap">
                <button type="button"
                    class="btn btn-outline-dark btn-skill border-0"
                    onmouseover="SkillPreview({0}, false);">
                    <img src="image/vmatrix/skill/skill_{0}_icon.png">
                </button>
                <button type="button"
                    class="btn btn-outline-dark btn-skill border-0"
                    onmouseover="SkillPreview({1}, false);">
                    <img src="image/vmatrix/skill/skill_{1}_icon.png">
                </button>
                <button type="button"
                    class="btn btn-outline-dark btn-skill border-0"
                    onmouseover="SkillPreview({2}, false);">
                    <img src="image/vmatrix/skill/skill_{2}_icon.png">
                </button>
            </div>
        </div>
        <div class="col-5 col-sm">
            <input id="core_{3}_level" class="form-control form-control-sm vertical-center" type="number" value="1" min="1" max="50" placeholder="레벨">
        </div>
        <div class="col-7 col-sm">
            <button type="button" class="btn btn-danger btn-sm btn-block vertical-center" onclick="RemoveCore({3});">삭제</button>
        </div>
    </div>
    `;

    console.log(selectedCoreSkill)
    coreList.push(selectedCoreSkill.slice());

    $('#core_list_panel').prepend(String.format(html, selectedCoreSkill[0], selectedCoreSkill[1], selectedCoreSkill[2], coreList.length - 1));
    $('#core_' + (coreList.length - 1) + '_panel').show(200);

    ResetSkillFromCore();

    $('#coreSelectedSkill').empty();
    StartPlaceButtonCheck();
}

function RemoveCore(id) {
    // coreList 에서 "특정" 요소 지우기..
    coreList.splice(id, 1);
    $('#core_' + id + '_panel').hide(200, function() {$(this).remove();});
    StartPlaceButtonCheck();
}

function ResetCore() {
    // RemoveCore는 Array의 크기를 변화시키므로 For문을 끝에서부터 돌림
    for (var x = coreList.length; x > -1; x--) {
        RemoveCore(x);
    }
    StartPlaceButtonCheck();
}

function StartPlaceButtonCheck() {
    if (coreList.length > 0) { $('#start-place-button').attr("disabled", false); }
    else { $('#start-place-button').attr("disabled", true); }
}

function SetOverlapCount(skill, overlap) {
    goalLength = skill.length * overlap;
    for (i in skill) {
        goal[skill[i]] = overlap;
    }
}

function GetArrayItems(arr, length, callback, oldindexList = []) {
    // array가 ref로 공유되는 특성이 있어서 새로운 Array로 복사
    var indexList = oldindexList.slice();

    // length개 만큼 선택되었을 때..
    if (indexList.length == length) {
        var result = [];
        for (x in indexList) {
            result.push(arr[indexList[x]]);
        }
        callback(result);
        return 0;
    }

    for (x in arr) {
        // 만약 x 가 indexList의 제일 큰 index보다 작으면 넘기기
        if (x <= indexList[indexList.length - 1]) { continue; }

        // indexList에 추가
        var newindexList = indexList.slice();
        newindexList.push(x);

        GetArrayItems(arr, length, callback, newindexList);
    }
}

function SortArrays(arr) {

    // Array.sort()
    // 음수 - a < b  : a-b 순서로 배치
    // 0    - a == b : 변동 x (a-b 순서로 배치)
    // 양수 - a > b  : b-a 순서로 배치

    arr.sort(
        function (a, b) {
            for (x in a) {
                if (a[x] != b[x]) { return (a[x] - b[x]); }
            }
            // 만약 배열의 길이가 다른 경우, 길이가 긴 쪽이 더 큰것으로 간주
            return a.length - b.length;
        }
    );
}

function isEqual(x, y) {
    const ok = Object.keys, tx = typeof x, ty = typeof y;
    return x && y && tx === 'object' && tx === ty ? (
        ok(x).length === ok(y).length &&
        ok(x).every(key => isEqual(x[key], y[key]))
    ) : (x === y);
}

function CheckCore(arr) {
    var item = [];
    var sum = {};

    for (x in arr) {
        // 첫번째 스킬은 "중복 불가"
        if (item.indexOf(arr[x][0]) != -1) { return; }
        item.push(arr[x][0]);

        // 총 스킬 강화 갯수 세기
        for (i in arr[x]) {
            if (typeof sum[arr[x][i]] === "undefined") { sum[arr[x][i]] = 1; }
            else { sum[arr[x][i]]++; }
        }
    }

    // 각 코어에 들어있는 스킬의 합이 목표치에 미달시 다음 코어로
    if (goalLength % 3 != 0) {
        if (!Object.keys(goal).every(key => isEqual(goal[key], sum[key]))) { return; }
    } else { if (!isEqual(sum, goal)) { return; } }

    coreCombinationResult.push(arr);
    return;
}

function StartPlace(overlap) {
    goal = {};
    goalLength = 0;
    coreCombinationResult = [];

    var modal = $('#coreResultModal .modal-body');
    modal.empty();

    SortArrays(coreList);
    SetOverlapCount(selectedSkill, overlap);
    // 3으로 나눠떨어지지 않는 경우 3의 배수로 올림
    GetArrayItems(coreList, Math.ceil(goalLength / 3), CheckCore);

    // 찾지 못한 경우
    if (coreCombinationResult.length == 0) {
        modal.html("<h5>이 코어들로는 요청한 조합을 만들 수 없었습니다...</h5>");
        return;
    } else {
        modal.html(String.format("<h5>{0}개의 조합을 찾았습니다!</h5>", coreCombinationResult.length));
        modal.append(`
    <div id="core-combination-panel" class="card rounded">
        <div class="card-header">
            <ul id="core-combination-tab" class="nav nav-pills card-header-pills" role="tablist"></ul>
        </div>
        <div class="tab-content" id="core-combination-content"></div>
    </div>`);
    }

    for(x in coreCombinationResult) {
        var content = "";

        for(y in coreCombinationResult[x]) {
            content += String.format(`
<li class="list-group-item">
    <div class="d-flex justify-content-center flex-wrap">
        <button type="button" class="btn btn-outline-dark btn-skill border-0">
            <img src="image/vmatrix/skill/skill_{0}_icon.png">
        </button>
        <button type="button" class="btn btn-outline-dark btn-skill border-0">
            <img src="image/vmatrix/skill/skill_{1}_icon.png">
        </button>
        <button type="button" class="btn btn-outline-dark btn-skill border-0">
            <img src="image/vmatrix/skill/skill_{2}_icon.png">
        </button>
        <h5 class="mx-3 my-auto">레벨:{3}</h5>
    </div>
</li>`, coreCombinationResult[x][y][0], coreCombinationResult[x][y][1], coreCombinationResult[x][y][2], 1);
        }

        $('#core-combination-tab').append(String.format(`
<li class="nav-item">
    <a class="nav-link{1}" id="core-combination-{0}-tab" data-toggle="tab"
        href="#core-combination-{0}" role="tab" aria-controls="core-combination-{0}"
        aria-selected="{2}">조합 {0}</a>
</li>`, parseInt(x) + 1, x == 0 ? " active" : "", x == 0 ? "true" : "false"));

        $('#core-combination-content').append(String.format(`
<ul id="core-combination-{0}" class="list-group list-group-flush tab-pane fade{2}"
    role="tabpanel" aria-labelledby="core-combination-{0}-tab">
    {1}
</ul>`, parseInt(x) + 1, content, x == 0 ? " show active" : ""));
    }
    
}