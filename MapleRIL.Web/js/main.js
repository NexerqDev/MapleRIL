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
        regions: window.MapleRIL.regions,
        topbarQuery: null,
        topbarQueryRegion: null
    },
    created: function () {
        this.topbarQueryRegion = this.regions[0].region;
    },
    methods: {
        topbarSearch: function () {
            if (!this.topbarQuery)
                return;
            window.location.href = `/search?q=${this.topbarQuery}&region=${this.topbarQueryRegion}`;
        }
    }
}).$mount("#app")