import Vue from "vue"
import VueRouter from "vue-router"

import HomeComponent from "./components/home.vue"
import SearchComponent from "./components/search.vue"
import LookupComponent from "./components/lookup.vue"

Vue.use(VueRouter)

const routes = [
    { path: "/", component: HomeComponent },
    { path: "/search", component: SearchComponent },
    { path: "/lookup", component: LookupComponent }
]

const router = new VueRouter({
    mode: "history", // dont use hashes, pure /search etc
    routes
})

const app = new Vue({
    router,
    data: {
        regions: window.MapleRIL.regions
    }
}).$mount("#app")