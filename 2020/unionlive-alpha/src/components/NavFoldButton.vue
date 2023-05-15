<template>
    <button @click="$emit('click')">
        <font-awesome-icon icon="bars" size="2x" />
    </button>
</template>

<script>
export default {
    name: "nav-fold-button",
    props: {},
};
</script>

<style scoped lang="scss">
@import "@/scss/_variables.scss";

button {
    margin: 0 10px;
    padding: 5px 10px;
    border: none;
    border-radius: 5px;
    font-family: $font-main;
    color: white;
    background-color: $color-100;
    transition: all 0.1s;
    cursor: pointer;

    &:hover {
        background-color: $color-200;
    }
}
</style>
