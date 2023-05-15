<template>
    <button v-bind:class="type" @click="$emit('click')">
        <slot></slot>
    </button>
</template>

<script>
export default {
    name: "soft-button",
    props: {
        type: String,
    },
};
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss">
@import "@/scss/_variables.scss";
@import "@/scss/_mixins.scss";

button {
    font-family: $font-main;
    padding: 5px 10px;
    border: none;
    border-radius: 5px;
    overflow: hidden;

    cursor: pointer;
    transition: 0.2s all;
    
    @include element-colors;
}
</style>
