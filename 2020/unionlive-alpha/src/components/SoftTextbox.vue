<template>
    <div id="search-panel" v-bind:class="type">
        <font-awesome-icon id="icon" icon="search" size="1x" />
        <input type="text"/>
    </div>
</template>

<script>
export default {
    name: "soft-textbox",
    props: {
        type: String,
    },
};
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss">
@import "@/scss/_variables.scss";
@import "@/scss/_mixins.scss";

#search-panel {
    padding: 5px 0;
    border: none;
    border-radius: 3px;
    overflow: hidden;
    display: flex;

    transition: 0.2s all;

    @include element-colors;

    * {
        margin-left: 10px;
    }

    input {
        width: 100%;
        border: none;
        background: none;
        padding: 0;
        font-family: $font-main;
        font-size: 18px;
        margin-right: 10px;
    }
}
</style>
