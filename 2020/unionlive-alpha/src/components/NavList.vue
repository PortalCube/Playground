<template>
    <div id="nav" @click="$emit('click')">
        <p id="logo"><img src="@/assets/union.png" @click="$emit('fold')" /></p>
        <nav-button-header text="게임 서비스" v-bind:src="image.princess" />
        <nav-button text="이벤트" destination="/events" />
        <nav-button text="에셋" destination="/" />
        <nav-button text="공지사항" destination="/" />
        <nav-button-header text="유틸리티" v-bind:src="image.fairy" />
        <nav-button text="PST 시뮬레이터" destination="/" />
        <nav-button-header text="기타" v-bind:src="image.angel" />
        <nav-button text="메인 페이지" destination="/" />
        <nav-button text="추가 정보" destination="/credit" />
        <nav-button text="테스트 페이지" destination="/sample" />
    </div>
</template>

<script>
import NavButton from "@/components/NavButton.vue";
import NavButtonHeader from "@/components/NavButtonHeader.vue";

export default {
    name: "nav-list",
    components: {
        NavButton,
        NavButtonHeader,
    },
    data: function() {
        return {
            image: {
                princess: require("@/assets/princess.png"),
                fairy: require("@/assets/fairy.png"),
                angel: require("@/assets/angel.png"),
            },
        };
    },
};
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss">
@import "@/scss/_variables.scss";
@import "@/scss/_mixins.scss";

#logo {
    margin: 0;
    text-align: center;
    padding: 15px;
    cursor: pointer;

    img {
        width: 360px;
    }
}

#nav {
    width: 100%;
    height: 100%;

    /* background setting */
    background-image: url("../assets/bg_l.png");
    background-repeat: no-repeat;
    background-position: center top;
    background-size: cover;
}
</style>
