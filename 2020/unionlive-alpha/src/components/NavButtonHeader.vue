<template>
    <p>
        <img v-bind:src="src" />
        <span> {{ text }}</span>
    </p>
</template>

<script>
export default {
    name: "nav-button-header",
    props: {
        src: String,
        text: String,
    },
};
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss">
@import "@/scss/_variables.scss";

p {
    margin: 0;
    padding: 10px 20px;
    font-weight: bold;
    font-size: 24pt;
    text-align: left;
    color: white;
}

span {
    font-family: $font-main;
}

img {
    width: 48px;
    vertical-align: middle;
}
</style>
