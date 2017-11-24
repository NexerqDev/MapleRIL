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
        let r = window.localStorage.getItem("region");
        if (r) {
            let s = this.regions.find(i => i.region === r)
            if (s) {
                this.topbarQueryRegion = s.region;
            } else {
                this.topbarQueryRegion = this.regions[0].region;
            }
        } else {
            this.topbarQueryRegion = this.regions[0].region;
        }

        let favs = window.localStorage.getItem("favorites");
        this.favorites = favs ? JSON.parse(favs) : [];

        // once we all loaded transition go go go
        document.body.className = "fade";
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
        },
        safeIcon: function (icon) { // "safely" does the icon - if null give placeholder
            if (!icon)
                return "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAA3ZpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuNi1jMTM4IDc5LjE1OTgyNCwgMjAxNi8wOS8xNC0wMTowOTowMSAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wTU09Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9tbS8iIHhtbG5zOnN0UmVmPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvc1R5cGUvUmVzb3VyY2VSZWYjIiB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iIHhtcE1NOk9yaWdpbmFsRG9jdW1lbnRJRD0ieG1wLmRpZDoxZmNlZWRiZC0yMzQ4LTFkNGQtYjRlMi1iNDg2MGQ4MTMzMDYiIHhtcE1NOkRvY3VtZW50SUQ9InhtcC5kaWQ6NzZGMjFERUNEMTVGMTFFNzlDRDc4NUI3NDhCOTUzNEQiIHhtcE1NOkluc3RhbmNlSUQ9InhtcC5paWQ6NzZGMjFERUJEMTVGMTFFNzlDRDc4NUI3NDhCOTUzNEQiIHhtcDpDcmVhdG9yVG9vbD0iQWRvYmUgUGhvdG9zaG9wIENDIDIwMTcgKFdpbmRvd3MpIj4gPHhtcE1NOkRlcml2ZWRGcm9tIHN0UmVmOmluc3RhbmNlSUQ9InhtcC5paWQ6MWZjZWVkYmQtMjM0OC0xZDRkLWI0ZTItYjQ4NjBkODEzMzA2IiBzdFJlZjpkb2N1bWVudElEPSJ4bXAuZGlkOjFmY2VlZGJkLTIzNDgtMWQ0ZC1iNGUyLWI0ODYwZDgxMzMwNiIvPiA8L3JkZjpEZXNjcmlwdGlvbj4gPC9yZGY6UkRGPiA8L3g6eG1wbWV0YT4gPD94cGFja2V0IGVuZD0iciI/Pt77LZ4AAAHmSURBVHja1Je/SgNBEMYvehKwS7RTNILYRlDfQIOIRTSVJI9gqb6A5hG0iaRJGhVSaqEPoGDAF/BPbRLrBJTzW5iTJc7eTnIrwYMfhL3Z+b7L3c7OJgJvtJc/QGwSbIIcWAULIE33PsAreAS34Ab0RFkDOylwDDogENKhOSlbfpt4EbQGEO5HzS0NY8AHlRjC/VQop8jAOGg4FA9pUG6rgfOIJD1QBwWQARNEhsbqFGOaf24zULQ8waLgo922mCiaDKTBOzPhExwFshWTFXy0LdL6ZaBsmHDoUDyk3G8gaVjnlw7Eu4Y6kdQN5A0TZ2OKq/GcwUReN3DKBNQciC9TXI25f6YbeGACCo7EFbtMzL1ugEs070jco1xc3E/AFxPgOxL3qFhxy3tgA8OIiwy0mYA5R+KRr2CM2oJnplVY035nwR2YZuLaYAM8RbQdK8zYi6cZaDIBeUfi6tphxpqeoBDlYvztITOSQmQqxd0I8aywTF9ISnHUZhRH/EC6GYXbccuh+D4tNfF2bGtIutRs2IRV03IVkadka8mqlpZMvdM9qhOqx5sES1TvbS1ZVdIT+n/YlPr/oi3XKY3qYKIzBU6GPJqlbfkTwWCH0y2wTvtERivPqiS/aYfTa+nhNDHq4/m3AAMA/AIRX7rUCXYAAAAASUVORK5CYII=";
            else
                return icon;
        },
        escapeHtml: function (str) {
            let div = document.createElement("div");
            div.appendChild(document.createTextNode(str));
            return div.innerHTML;
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