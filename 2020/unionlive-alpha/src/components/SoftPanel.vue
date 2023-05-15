<template>
    <div id="softpanel" key="softpanel">
        <slot></slot>
    </div>
</template>

<script>
export default {
    name: "soft-panel",
    props: {},
};
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss">
@import "@/scss/_variables.scss";

#softpanel {
    margin: 10px;
    padding: 10px;
    border: 1px solid $color-800;
    border-radius: 5px;
    box-shadow: 0 0 5px #0000001a;
}
</style>
