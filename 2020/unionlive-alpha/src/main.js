import Vue from "vue";
import App from "./App.vue";
import router from "./router";

// Font Awesome Icons

import { library } from "@fortawesome/fontawesome-svg-core";

import { faBars, faSearch } from "@fortawesome/free-solid-svg-icons";

import {
    FontAwesomeIcon,
    FontAwesomeLayers,
    FontAwesomeLayersText,
} from "@fortawesome/vue-fontawesome";

library.add(faBars, faSearch);

Vue.component("font-awesome-icon", FontAwesomeIcon);
Vue.component("font-awesome-layers", FontAwesomeLayers);
Vue.component("font-awesome-layers-text", FontAwesomeLayersText);

Vue.config.productionTip = true;

new Vue({
    router,
    render: (h) => h(App),
}).$mount("#app");
