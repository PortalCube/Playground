<template>
    <router-link v-bind:to="destination">
        <button type="button">{{ text }}</button>
    </router-link>
</template>

<script>
export default {
    name: "",
    props: {
        destination: String,
        text: String
    }
};
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss">
@import "@/scss/_variables.scss";

button {
    width: 100%;
    padding: 10px 75px;
    border: none;
    font-size: 18pt;
    text-align: left;
    background-color: #ffffff00;
    color: white;
    cursor: pointer;
    &:hover {
        background-color: #ffffff11;
    }
}
</style>
