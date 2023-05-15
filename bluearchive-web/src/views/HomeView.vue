<script setup lang="ts">
import StudentComponent from "../components/roadmap/StudentComponent.vue";
import image1 from "../assets/portrait/1.png";
import image2 from "../assets/portrait/2.png";
import image3 from "../assets/portrait/3.png";
import image4 from "../assets/portrait/4.png";
import image5 from "../assets/portrait/5.png";
console.log(image1);
</script>

<template>
    <main>
        <StudentComponent
            name="아즈사"
            skin="수영복"
            :image="image1"
            :star="3"
            type="striker"
            draw="limited"
            :is-new="true"
        />
        <StudentComponent
            name="미카"
            skin=""
            :image="image2"
            :star="3"
            type="striker"
            draw="fes"
            :is-new="true"
        />
        <StudentComponent
            name="미야코"
            skin=""
            :image="image5"
            :star="3"
            type="striker"
            draw="normal"
            :is-new="false"
        />
        <StudentComponent
            name="하츠네 미쿠"
            skin=""
            :image="image3"
            :star="3"
            type="special"
            draw="special"
            :is-new="true"
        />
        <StudentComponent
            name="하나코"
            skin=""
            :image="image4"
            :star="3"
            type="special"
            draw="normal"
            :is-new="false"
        />
    </main>
</template>

<style scoped>
main {
    background-color: white;
    width: 100%;
    height: 600px;
    padding: 12px;
    display: flex;
    flex-wrap: wrap;
    align-content: flex-start;
    /* justify-content: space-between; */
    overflow: auto;
}

main > * {
    margin: 12px;
}
</style>
