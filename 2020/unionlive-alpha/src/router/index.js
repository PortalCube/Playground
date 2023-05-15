import Vue from "vue";
import VueRouter from "vue-router";
import Home from "../views/Home.vue";

Vue.use(VueRouter);

const routes = [
    {
        path: "/sample",
        name: "sample",
        component: () =>
            import(/* webpackChunkName: "sample" */ "../views/Sample.vue"),
    },
    {
        path: "/events",
        name: "events",
        component: () =>
            import(/* webpackChunkName: "sample" */ "../views/Events.vue"),
    },
    {
        path: "/credit",
        name: "credit",
        component: () =>
            import(/* webpackChunkName: "sample" */ "../views/Credit.vue"),
    },
    {
        path: "*",
        name: "anywhere",
        component: Home,
    },
];

const router = new VueRouter({
    mode: "history",
    base: process.env.BASE_URL,
    routes,
});

export default router;
