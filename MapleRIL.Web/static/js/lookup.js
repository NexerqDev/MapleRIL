Vue.prototype.$http = axios;

var app = new Vue({
    el: '#app',
    data: {
        queryId: window.MapleRIL.id,
        region: window.MapleRIL.region,
        regions: [],
        invalid: false,
        loading: true,
        sourceRegion: null,
        targetRegion: null,
        sourceData: null,
        targetData: null
    },
    created: function () {
        if (!this.queryId || !this.region) {
            this.invalid = true;
            this.loading = false;
            return;
        }

        // get region data then go from there
        this.$http.get('/api/regions')
            .then(resp => {
                this.regions = resp.data.regions;

                // invalid region
                var region = this.regions.find(r => r.region === this.region);
                if (!region) {
                    this.invalid = true;
                    this.loading = false;
                    return;
                }

                this.sourceRegion = region;

                // get source data
                this.lookupInRegion(this.region)
                    .then(d => this.sourceData = d)
                    .then(() => this.loading = false);
            });
    },
    methods: {
        lookupInRegion: function (regionName) {
            return this.$http.get(`/api/item?id=${this.queryId}&region=${regionName}`)
                .then(resp => {
                    if (resp.data.error && resp.data.error._errid === "NO_ITEM")
                        return null;

                    return resp.data.item;
                });
        }
    }
});