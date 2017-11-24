import Vue from "vue";
import VueRouter from "vue-router";

import HomeComponent from "./components/home.vue";
import SearchComponent from "./components/search.vue";
import SearchLookupComponent from "./components/searchLookup.vue";
import AboutComponent from "./components/about.vue";
import FavoritesComponent from "./components/favorites.vue";
import NotFoundComponent from "./components/404.vue";

Vue.use(VueRouter);

const routes = [
    { path: "/", component: HomeComponent },
    { path: "/search", component: SearchComponent },
    { path: "/search/lookup", component: SearchLookupComponent },
    { path: "/about", component: AboutComponent },
    { path: "/favorites", component: FavoritesComponent },
    { path: "*", component: NotFoundComponent } // 404 catch all
];

const router = new VueRouter({
    mode: "history", // dont use hashes, pure /search etc
    routes
});

// eslint-disable-next-line no-unused-vars
const app = new Vue({
    router,
    data: {
        regions: window.MapleRIL.regions,
        topbarQuery: null,
        topbarQueryRegion: null,
        transitionName: "fade",
        favorites: null
    },
    created: function () {
        this.topbarQueryRegion = window.localStorage.getItem("region") || this.regions[0].region;

        let favs = window.localStorage.getItem("favorites");
        this.favorites = favs ? JSON.parse(favs) : [];
    },
    methods: {
        topbarSearch: function () {
            if (!this.topbarQuery)
                return;
            window.localStorage.setItem("region", this.topbarQueryRegion);
            this.$router.push({ path: "/search", query: { q: this.topbarQuery, region: this.topbarQueryRegion } });
        },
        saveFavorites: function () {
            window.localStorage.setItem("favorites", JSON.stringify(this.favorites));
        }
    },
    watch: {
        // route watcher to do slide/fades
        "$route": function (to, from) {
            const fromDepth = from.path.split("/").length;
            const toDepth = to.path === "/" ? 1 : to.path.split("/").length;
            this.transitionName = toDepth < fromDepth ? "slide-right" : "slide-left";
        }
    }
}).$mount("#app");