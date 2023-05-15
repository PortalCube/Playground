<script setup lang="ts">
import { ref, onMounted, reactive } from "vue";

// 학생 레어리티 및 타입 과 뽑기 타입 간의 애니메이션

// 1. A 디스플레이 온 (레어리티, 타입)
//  - 종료 애니메이션을 실행할 타이머 온
// 2. 종료 애니메이션 타이머 온
//  - 종료 애니메이션 실행, 애니메이션 리스너 달아두셈
// 3. 종료 리스너 온
//  - 현재 요소에 hidden 클래스 부착. B 디스플레이에 hidden 클래스 제거.
// 1. B 디스플레이 온 (뽑기 타입)
//  - 종료 애니메이션을 실행할 타이머 온...

// ---------------------------------------

export type StudentDraw = "normal" | "gift" | "limited" | "special" | "fes";

const statusOne = ref<HTMLDivElement>();
const statusOneAlt = ref<HTMLDivElement>();
const statusTwo = ref<HTMLDivElement>();

let status = true;

const statusOneClass = reactive({
    fadeout: false,
    fadein: false,
    hidden: false
});
const statusTwoClass = reactive({
    fadeout: false,
    fadein: false,
    hidden: true
});

const showTime = 4000;
const animationEndTrigger = (element: HTMLDivElement) => {
    element.onanimationend = () => {
        status = !status;
        statusOneClass.hidden = !status;
        statusTwoClass.hidden = status;
        if (status) {
            statusOneClass.fadeout = false;
            statusOneClass.fadein = true;
        } else {
            statusTwoClass.fadeout = false;
            statusTwoClass.fadein = true;
        }
        element.onanimationend = null;
    };
};
const animationRepeat = () => {
    let element = null;
    if (status) {
        statusOneClass.fadein = false;
        statusOneClass.fadeout = true;
        if (statusOne.value) {
            element = statusOne.value;
        }
    } else {
        statusTwoClass.fadein = false;
        statusTwoClass.fadeout = true;
        if (statusTwo.value) {
            element = statusTwo.value;
        }
    }
    if (element) {
        animationEndTrigger(element);
        setTimeout(animationRepeat, showTime);
    }
};
setTimeout(animationRepeat, showTime);

const drawText = {
    normal: "상시 모집",
    gift: "이벤트 배포",
    limited: "기간 한정",
    special: "특별 한정",
    fes: "페스 한정"
};

defineProps<{
    name: string;
    skin: string;
    image: string;
    star: number;
    type: string;
    draw: StudentDraw;
    isNew: boolean;
}>();
</script>

<template>
    <div class="wrapper">
        <div class="portrait" :style="{ backgroundImage: `url(${image})` }">
            <div class="new-icon" :class="{ visible: isNew }">
                <p class="new-text">NEW!</p>
            </div>
            <div class="student-info">
                <div
                    class="student-star"
                    :class="statusOneClass"
                    v-text="'★'.repeat(star)"
                    ref="statusOneAlt"
                />
                <div
                    class="student-type"
                    :class="[type, statusOneClass]"
                    v-text="type.toUpperCase()"
                    ref="statusOne"
                />
                <div
                    class="student-draw-type"
                    :class="[draw, statusTwoClass]"
                    v-text="drawText[draw]"
                    ref="statusTwo"
                />
            </div>
        </div>
        <p class="name" v-text="name" />
        <p class="name skin" v-text="`(${skin})`" v-if="skin" />
    </div>
</template>

<style scoped>
.wrapper {
    width: 130px;
    cursor: pointer;
}

.portrait {
    width: 130px;
    height: 146px;
    display: flex;
    flex-direction: column;
    justify-content: space-between;
    box-shadow: 0px 1px 6px #0000003f;
    background-image: url(@/assets/portrait.png);
    background-position: center;
    background-size: 100%;
    border-radius: 10px;
    color: white;
    overflow: hidden;
    transition: background-size 0.25s;
}

.portrait:hover {
    background-size: 133%;
}

.wrapper:hover > .portrait {
    background-size: 133%;
}

.new-icon {
    width: 64px;
    height: 64px;
    margin-top: -32px;
    margin-left: -32px;
    display: flex;
    justify-content: center;
    align-items: flex-end;
    background-color: #ff4343;
    transform: rotate(315deg);
    font-size: 9pt;
    line-height: 14pt;
    animation-duration: 1s;
    animation-name: upanddown;
    animation-direction: alternate;
    animation-iteration-count: infinite;
    animation-timing-function: ease-in-out;
    visibility: hidden;
}

.new-icon.visible {
    visibility: visible;
}

@keyframes upanddown {
    from {
        font-size: 9pt;
    }
    to {
        font-size: 8pt;
    }
}

.student-info {
    height: 18px;
    display: flex;
    justify-content: center;
    align-items: center;
    background-color: #ffffffff;
}

@keyframes fadeout {
    from {
        opacity: 1;
    }

    to {
        opacity: 0;
    }
}

@keyframes fadein {
    from {
        opacity: 0;
    }

    to {
        opacity: 1;
    }
}

.student-info > *.fadeout {
    animation-duration: 1s;
    animation-name: fadeout;
}
.student-info > *.fadein {
    animation-duration: 1s;
    animation-name: fadein;
}
.student-info > *.hidden {
    display: none;
}

.student-star {
    color: #fbb113;
    margin-right: 8px;
    font-size: 10pt;
    letter-spacing: 1px;
}
.student-type {
    margin-top: 2px;
    font-size: 9pt;
}

.student-type.striker {
    color: #ff0000;
}
.student-type.special {
    color: #005aff;
}

.student-draw-type {
    color: #222222;
    font-size: 10pt;
}
.student-draw-type.gift {
    color: #a14800;
}
.student-draw-type.limited {
    color: #ff0000;
}
.student-draw-type.special {
    color: #ff7a0f;
}
.student-draw-type.fes {
    color: #e403ff;
}

.name {
    margin-top: 10px;
    font-size: 20pt;
    line-height: 20pt;
    color: black;
    text-align: center;
}

.name.skin {
    margin-top: 3px;
    font-size: 10pt;
    font-weight: 200;
    line-height: 10pt;
}
</style>
