const list = [
    "사아 이마오카가야케!!",
    "자 지금을 빛내자!!",
    "예스 이쿠츠모노 오모이데타치",
    "예스 수많은 추억들을",
    "마부시이 쿄오노 히카리에",
    "눈부신 오늘의 빛으로 바꾸어",
    "사아 이마오하바타케!!",
    "자 지금을 날아보자!!",
    "호코라시이 쿄오노 츠바사에",
    "자랑스런 오늘의 날개로 바꾸어",
    "아타라시이 마쿠오아케요오",
    "새로운 막을 열어 보자",
    "유메오 하지메테네갓테",
    "꿈을 처음 바라고서",
    "쿄오마데 도노쿠라이탓타 다로오",
    "오늘까지 얼마나 지나온 걸까",
    "즛토 이치니치즈츠 츠나게요오",
    "계속 하루하루를 이어가 보자",
    "유메와 지분오 카나에루타메니",
    "꿈이란 자신을 이뤄내기 위해",
    "우마레타 아카시다카라",
    "태어난 증표니까",
    "킷토 코노 코코로데",
    "반드시 이 마음으로",
    "얏또 히라쿠 타카라바코오",
    "힘겹게 열어본 보물상자를",
    "키미토 노조케바",
    "너와 함께 들여다보면",
    "아타라시이 세카이노 소라",
    "새로운 세상의 하늘",
    "아타라시이 우타오노코소",
    "새로운 노래를 남기자",
    "히토리자 카케나이",
    "혼자서는 쓸 수 없는",
    "츠나기츠즈쿠 메로디",
    "계속해서 이어지는 멜로디",
    "코코카라",
    "여기서부터",
    "긴가노 하즈레가 키라메이타토 시타라",
    "은하의 가장자리가 반짝였다면",
    "이마 호라 코노코에 토도이타",
    "지금 자 목소리가 닿은거야",
    "아시아토 나라베요 카쿠후니 노코스요오",
    "발걸음을 맞춰보자 악보에 남기기 위해",
    "네가이 와라이 리즈무와 하샤기다스",
    "바라고 웃으며 리듬은 들떠올라",
    "마호 마톳테쿠 테노히라",
    "마법에 휘감기는 손바닥",
    "미라이 리소오 이조오 히비키 아이마스요오니",
    "미래 이상 이상 함께 울려퍼지도록",
    "사아 우타오",
    "자 노래하자",
    "유메오 코에테 키세키오 코에테 카가야케루 이마",
    "꿈을 넘어서 기적을 넘어서 빛날 수 있는 지금",
    "젠아이 젠슌칸오",
    "모든 사랑 모든 순간을",
    "카나에테쿳떼 미라이케이니 토키메쿠 이마오",
    "'이루어가겠다'고 미래계에 고동치는 지금을",
    "다키시메요오",
    "품에 끌어안자",
    "코코다요",
    "여기야",
    "이츠모노 에가오가",
    "늘상 짓던 미소가",
    "다이조부오 쿠레루",
    "용기를 북돋아주었어",
    "코노 이마 나니가 유키카우노",
    "바로 지금 무엇이 오가는 걸까?",
    "세옷따리모 시타케도",
    "힘든 일도 있었지만",
    "토부타메니 아룬다",
    "날아오르기 위해 있었어",
    "키보오 카코 야쿠소쿠 아사야케",
    "희망 과거 약속 아침놀",
    "오타가이 젠젠 핀토코나이",
    "서로 전혀 와닿지 않는",
    "오모이데호도 아타타카이",
    "추억일수록 따뜻해",
    "이놋따 분 하시루요 와라이아에루 요오니",
    "빌었던 만큼 달려나갈게 함께 웃을 수 있도록",
    "오오조라에 후토시타 오오키나 유메",
    "드넓은 하늘로 별거 아닌 커다란 꿈들을",
    "마타 후리마코",
    "다시 흩뿌리자",
    "아노 코로 카이타 미라이니 아테타",
    "그 시절에 썼던 미래를 향해 보낸",
    "테가미타치모 코코에 후리소소이다",
    "편지들도 여기에 쏟아져 내렸어",
    "아타라시이 타카라바코에",
    "새로운 보물상자에",
    "츠메코은데 유코오",
    "한가득 담아가자",
    "나은데모나이 메로디 사아",
    "별것도 아닌 멜로디를 자",
    "이츠마데모 아후레루 요오나",
    "언제까지고 넘쳐흐를 것 같은",
    "츠메키레나이 요오나",
    "다 담아가지 못할 것 같은",
    "토와니 츠즈쿠 하모니",
    "영원히 이어지는 하모니",
    "우타이츠즈케요",
    "계속 노래하자",
    "유메오 코에테 키세키오 코에테 카가야케루 이마",
    "꿈을 넘어서 기적을 넘어서 빛날 수 있는 지금",
    "카나에테쿳떼 미라이케이니 토키메쿠 이마오",
    "'이루어가겠다'고 미래계에 고동치는 지금을",
    "다키시메요오",
    "품에 끌어안자",
    "토마라나이 모노가타리데",
    "멈추지 않는 이야기로",
    "코에 츠나이데 우타이아오 코코다요",
    "목소리를 이어가며 함께 노래하자 여기야",
    "코노 토키 코코카라",
    "이 시간 여기서부터",
    "코노 마마",
    "이대로 쭉",
    "미아게타아케노소라니오모이카사네",
    "올려다본 동트는 하늘에 마음을 포개어",
    "타쿠상노오모이데타치가세나카오스노",
    "수많은 추억들이 등을 떠밀어주는 걸",
    "아타라시이세카이와모우메노마에히로갓떼탄다",
    "새로운 세상은 이미 눈앞에 펼쳐져 있었어",
    "카와라나이오모이와미라이에노파스포-토",
    "변하지 않는 마음은 미래를 향한 패스포트",
    "맛떼테네유메오코에타유메",
    "기다려줘 꿈을 넘어선 꿈",
    "츠카미니유코우",
    "붙잡으러 가자",
    "하루카나소라가이로즈쿠요 오모이노하나히라쿠요우니",
    "드넓은 하늘이 여물어가 마음의 꽃을 펼치는 것처럼",
    "키보우노히카리아후레다시테치즈오소메테유쿠",
    "희망의 빛이 흘러나오며 지도를 물들여가",
    "젠료쿠데카가야키타이 키노우요리아시타못또",
    "전력으로 빛나고 싶어 어제보다 내일 더욱",
    "토도케타이도코마데모유메오츠나구요",
    "도달하고픈 어디까지고 꿈을 이어갈 거야",
    "무겐노소라니난도데모",
    "무한한 하늘에 몇번이고",
    "키라메쿠유메오에가이테유코우",
    "빛나는 꿈을 그려나가기로 하자",
    "코노칸도우모아이죠우모미라이오이로도루",
    "이 감동도 애정도 미래를 수놓는",
    "토도케타이도코마데모유메오츠나고우",
    "도달하고픈 어디까지고 꿈을 이어가자"
];

function ShuffleArray(arr) {
    for (let index = arr.length - 1; index > 0; index--) {
        const randomPosition = Math.floor(Math.random() * (index + 1));
        const temp = arr[index];
        arr[index] = arr[randomPosition];
        arr[randomPosition] = temp;
    }
    return arr;
}

window.quizList = ShuffleArray(list);
