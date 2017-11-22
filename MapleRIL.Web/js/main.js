import Vue from "vue"
import VueRouter from "vue-router"

import HomeComponent from "./components/home.vue"
import SearchComponent from "./components/search.vue"
import SearchLookupComponent from "./components/searchLookup.vue"
import AboutComponent from "./components/about.vue"
import NotFoundComponent from "./components/404.vue"

Vue.use(VueRouter)

const routes = [
    { path: "/", component: HomeComponent },
    { path: "/search", component: SearchComponent },
    { path: "/search/lookup", component: SearchLookupComponent },
    { path: "/about", component: AboutComponent },
    { path: "*", component: NotFoundComponent } // 404 catch all
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
        topbarQueryRegion: null,
        transitionName: "fade"
    },
    created: function () {
        this.topbarQueryRegion = window.localStorage.getItem("region") || this.regions[0].region;
    },
    methods: {
        topbarSearch: function () {
            if (!this.topbarQuery)
                return;
            window.localStorage.setItem("region", this.topbarQueryRegion);
            this.$router.push({ path: "/search", query: { q: this.topbarQuery, region: this.topbarQueryRegion } });
        }
    },
    watch: {
        // route watcher to do slide/fades
        "$route": function (to, from) {
            const fromDepth = from.path.split('/').length
            const toDepth = to.path === "/" ? 1 : to.path.split('/').length
            this.transitionName = toDepth < fromDepth ? 'slide-right' : 'slide-left'
        }
    }
}).$mount("#app")