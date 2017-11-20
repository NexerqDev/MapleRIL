Vue.prototype.$http = axios;

var app = new Vue({
    el: '#app',
    data: {
        queryId: window.MapleRIL.id,
        region: window.MapleRIL.region,
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
            return;
        }
    },
    methods: {
        lookupInRegion: function (e) {
            // y
        }
    }
});