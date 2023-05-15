<template> </template>

<script>
export default {
    name: "simple-button",
    props: {
        id: {
            type: [String, Number],
            default: 0
        },
        type: String
    }
};
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss">
@import "@/scss/_variables.scss";

button {
    font-family: $font-main;
    border: none;
    color: white;
    padding: 14px 28px;
    cursor: pointer;

    &.success {
        background-color: #4caf50;
        color: black;
        &:hover {
            background-color: #46a049;
        }
    } /* Green */

    &.info {
        background-color: #2196f3;
        &:hover {
            background: #0b7dda;
        }
    } /* Blue */

    &.warning {
        background-color: #ff9800;
        &:hover {
            background: #e68a00;
        }
    } /* Orange */

    &.danger {
        background-color: #f44336;
        &:hover {
            background: #da190b;
        }
    } /* Red */

    &.default {
        background-color: #e7e7e7;
        color: black;
        &:hover {
            background: #ddd;
        }
    } /* Gray */
}
</style>
