<template>
    <div>
        <soft-panel id="panel">123</soft-panel>
    </div>
</template>

<script>
// @ is an alias to /src
import SoftPanel from "@/components/SoftPanel.vue";

export default {
    name: "events",
    components: {
        SoftPanel,
    },
};
</script>

<style scoped lang="scss">
@import "@/scss/_variables.scss";
@import "@/scss/_mixins.scss";

#panel {
    margin: 10px auto;
    max-width: 800px;
}
</style>
