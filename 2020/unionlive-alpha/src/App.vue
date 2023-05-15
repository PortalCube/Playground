<template>
    <div id="app">
        <transition name="slide-left">
            <nav-list
                key="nav"
                name="nav"
                id="nav"
                v-bind:class="{ absolute: navSmall }"
                v-show="navShow"
                v-on:fold="foldNav()"
                v-on:click="clickNav()"
            />
        </transition>
        <div id="main" key="content">
            <div id="banner">
                <p>
                    <nav-fold-button id="fold-btn" v-on:click="foldNav()" />{{
                        directory
                    }}
                </p>
            </div>
            <div id="content">
                <transition name="fade" mode="out-in">
                    <router-view id="router" />
                </transition>
            </div>
        </div>
    </div>
</template>

<script>
const _ = require("lodash");

// Components
import NavList from "@/components/NavList.vue";
import NavFoldButton from "@/components/NavFoldButton.vue";

export default {
    name: "app",
    components: {
        NavList,
        NavFoldButton,
    },
    data: function() {
        return {
            directory: "메인 페이지",
            windowWidth: window.innerWidth,
            navSmall: window.innerWidth < 1200,
            navShow: !(window.innerWidth < 1200),
        };
    },
    mounted: function() {
        window.addEventListener("resize", _.debounce(this.updateWidth, 300));
    },
    methods: {
        updateWidth: function() {
            this.windowWidth = window.innerWidth;
            this.navSmall = this.windowWidth < 1200;
            this.navShow = !this.navSmall;
        },
        foldNav: function() {
            return (this.navShow = !this.navShow);
        },
        clickNav: function() {
            if (this.navShow && this.navSmall) {
                this.foldNav();
            }
        },
    },
};
</script>

<style lang="scss">
@import "@/scss/_variables.scss";
@import "@/scss/_mixins.scss";

* {
    font-family: $font-main;
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;
}

p,
span,
li {
    font-family: $font-sub;
}
</style>

<style scoped lang="scss">
@import "@/scss/_variables.scss";
@import "@/scss/_mixins.scss";

#app {
    @include clearfix;
    display: flex;

    /* font setting */
    font-family: $font-sub;
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;

    /* text setting */
    color: $color-black;
}

.absolute {
    position: absolute;
}

#nav {
    transition: all 0.5s;
    max-width: 400px;
    overflow: auto;
    z-index: 100;
}

#main {
    width: 100%;
    max-width: 100%;
    display: flex;
    flex-direction: column;
    transition: all 0.5s;
}

#content {
    height: 100%;
    overflow: auto;
}

#banner {
    background-color: $color-100;
    font-family: $font-main;
    text-align: left;
    color: $color-white;
    padding: 10px 5px;

    p {
        font-family: $font-main;
        font-size: 24pt;
        font-weight: bold;
        vertical-align: center;
        margin: 0;
    }
}

.slide-left-enter-active {
    transition: margin 0.5s;
}

.slide-left-leave-active {
    transition: margin 0.2s;
}

.slide-left-enter,
.slide-left-leave-to {
    margin-left: -400px;
}

.fade-enter-active {
    transition: opacity 0.5s;
}

.fade-leave-active {
    transition: opacity 0.1s;
}

.fade-enter,
.fade-leave-to {
    opacity: 0;
}
</style>
