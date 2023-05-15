<script setup lang="ts">
import { RouterLink, RouterView } from "vue-router";
import AppHeader from "./components/AppHeader.vue";
import AppFooter from "./components/AppFooter.vue";
</script>

<template>
    <video
        src="@/assets/background.mp4"
        type="video/mp4"
        autoplay
        loop
        muted
        class="background"
    />

    <AppHeader />

    <div class="view">
        <RouterView />
    </div>

    <AppFooter />
</template>

<style scoped>
.view {
    max-width: 1280px;
    min-height: calc(100vh - 90px);
    margin: auto;
    padding-top: 90px;
    display: flex;
}

.view > * {
    margin: auto;
}

.background {
    position: fixed;
    left: 0;
    top: 0;
    width: 100%;
    height: 100%;
    object-fit: cover;
}
</style>
