Vue.prototype.$http = axios;

var app = new Vue({
    el: '#app',
    data: {
        query: window.MapleRIL.query,
        region: window.MapleRIL.region,
        regions: [],
        lookup: [],
        emptyLoad: false,
        searching: false
    },
    created: function () {
        if (!this.query || !this.region)
            this.emptyLoad = true;

        this.$http.get('/api/regions')
            .then(resp => {
                this.regions = resp.data.regions;

                if (!this.region)
                    this.region = this.regions[0].region;
            });

        this.search();
    },
    methods: {
        search: function (e) {
            if (!this.query || !this.region)
                return;

            this.searching = true;
            this.$http.get(`/api/search?q=${this.query}&region=${this.region}`)
                .then(resp => {
                    this.lookup = resp.data.items;
                    this.emptyLoad = false;
                    this.searching = false;

                    if (window.history.pushState)
                        window.history.pushState({}, "MapleRIL - Search", `?q=${this.query}&region=${this.region}`); // update browser url
                })
                .catch(e => this.searching = false);
        },
        redirLookup: function (item) {
            window.location.href = `/lookup?id=${item.id}&region=${this.region}`;
        }
    }
});