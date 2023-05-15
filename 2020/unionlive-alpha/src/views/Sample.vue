<template>
    <soft-panel>
        <h1>장비를 실험하겠습니다.</h1>
        <soft-button class="btn" type="light"><p>Light</p></soft-button>
        <soft-button class="btn" type="gray"><p>Gray</p></soft-button>
        <soft-button class="btn" type="dark"><p>Dark</p></soft-button>
        <soft-button class="btn" type="red"><p>Red</p></soft-button>
        <soft-button class="btn" type="green"><p>Green</p></soft-button>
        <soft-button class="btn" type="blue"><p>Blue</p></soft-button>
        <soft-button class="btn" type="yellow"><p>Yellow</p></soft-button>
        <soft-button class="btn" type="purple"><p>Purple</p></soft-button>
        <soft-button class="btn" type="orange"><p>Orange</p></soft-button>
        <soft-button class="btn" type="pink"><p>Pink</p></soft-button>
        <soft-button class="btn" type="teal"><p>Teal</p></soft-button>
        <soft-textbox type="gray"></soft-textbox>
    </soft-panel>
</template>

<script>
// @ is an alias to /src
import SoftPanel from "@/components/SoftPanel.vue";
import SoftButton from "@/components/SoftButton.vue";
import SoftTextbox from "@/components/SoftTextbox.vue";

export default {
    name: "sample",
    components: {
        SoftPanel,
        SoftButton,
        SoftTextbox,
    },
    data: function() {
        return {
            type: "info",
        };
    },
    methods: {
        message: function(msg) {
            alert(msg);
        }
    },
};
</script>

<style scoped lang="scss">
@import "@/scss/_variables.scss";

.btn {
    margin: 10px;
}

p {
    width: 100px;
    margin: 0;
    text-align: left;
    font-family: $font-main;
    font-size: 18px;
    font-weight: bold;
}
</style>
