<script setup lang="ts"></script>

<template>
    <header>
        <img class="logo" src="@/assets/logo.png" alt="블루 아카이브" />

        <nav class="navList">
            <RouterLink to="/">로드맵</RouterLink>
            <RouterLink to="/about">????</RouterLink>
        </nav>

        <div class="navButton">
            <img src="@/assets/icon/down-arrow.png" alt="메뉴 표시하기" />
        </div>
    </header>
</template>

<style scoped>
header {
    top: 0px;
    width: 100%;
    height: 90px;
    padding: 20px 0;
    z-index: 999;
    position: fixed;
    display: flex;
    background: rgb(255, 255, 255);
    background: linear-gradient(
        90deg,
        rgba(255, 255, 255, 0.75) 60%,
        rgba(255, 255, 255, 0) 100%
    );
    backdrop-filter: blur(5px);
    box-shadow: 0px 0px 12px #0000002f;
}

.logo {
    margin-left: 80px;
    margin-right: 40px;
    height: 50px;
    object-fit: contain;
}

.navButton {
    padding: 8px;
    margin-right: 10px;
    display: none;
    cursor: pointer;
    background: rgba(255, 255, 255, 0.95);
    backdrop-filter: blur(5px);
    box-shadow: 0px 0px 12px #0000002f;
    border-radius: 100%;
    align-self: flex-end;
}

.navButton img {
    margin-bottom: -8px;
    height: 36px;
}

.navList {
    font-size: 24pt;
    display: flex;
}

.navList * {
    margin: 0 20px;
    border-radius: 4px;
    color: black;
    background-color: #ffffff00;
    text-decoration: none;
    transition: color 0.15s;
}

.navList *:hover {
    color: rgb(45, 143, 255);
}

@media (max-width: 768px) {
    header {
        background: rgba(255, 255, 255, 0.7);
        flex-direction: column;
    }

    .logo {
        margin: 0;
        margin-bottom: 30px;
    }

    .navButton {
        display: block;
    }

    .navList {
        margin-bottom: 10px;
        flex-direction: column;
        align-items: stretch;
    }

    .navList * {
        margin: 8px 10px;
        text-align: center;
        background: rgba(255, 255, 255, 0.95);
        backdrop-filter: blur(5px);
        box-shadow: 0px 0px 12px #0000002f;
    }
}
</style>
