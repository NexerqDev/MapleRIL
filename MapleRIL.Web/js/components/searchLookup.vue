<template>
    <div id="lookup">
        <div v-if="loading" class="text-center mt-2">
            <i class="fa fa-spinner fa-pulse fa-3x fa-fw"></i>
            <span class="sr-only">Loading...</span>
        </div>
        <div v-else="">
            <div v-if="invalid">
                <div class="alert alert-dismissible alert-danger">
                    <strong>Invalid lookup.</strong> <a href="#" onclick="window.location.back();">Click here to return to the previous page.</a>.
                </div>
            </div>
            <div v-else="">
                <h2>Lookup ID: {{ queryId }}</h2>

                <div class="row">
                    <div class="col-lg-6">
                        <div class="card text-center">
                            <h3 class="card-header">
                                {{ sourceRegion.region }} ({{ sourceRegion.version }})
                                <span class="pull-right">
                                    <i v-if="favorited" class="fa fa-star" aria-hidden="true" v-on:click="favorite(false)" style="cursor: pointer; color: #ffc300;"></i>
                                    <i v-else class="fa fa-star-o" aria-hidden="true" v-on:click="favorite(true)" style="cursor: pointer;"></i>
                                </span>
                            </h3>
                            <img class="mt-2" style="height: 50px; object-fit: contain; display: block; width: 100%;" :src="$root.safeIcon(sourceData.icon)" :alt="sourceData.name">
                            <div class="card-body">
                                <h5 class="card-title">{{ sourceData.name }}</h5>
                                <h6 class="card-subtitle text-muted">{{ sourceData.category }}</h6>
                            </div>
                            <div class="card-body" style="padding-top: 0;">
                                <p class="card-text">
                                    <pre style="font-family: inherit; white-space: pre-wrap; overflow-x: hidden;" v-html="parseDesc(sourceData.description)"></pre>
                                </p>
                            </div>
                            <div class="card-footer text-muted">
                                This is the source region&#39;s item.
                            </div>
                        </div>
                    </div>

                    <div class="col-lg-6">
                        <div class="card text-center">
                            <!-- Target region selected & loaded -->
                            <div v-if="targetRegion">
                                <h3 class="card-header">{{ targetRegion.region }} ({{ targetRegion.version }})</h3>

                                <div v-if="targetData">
                                    <!-- have data -->
                                    <img class="mt-2" style="height: 50px; object-fit: contain; display: block; width: 100%;" :src="$root.safeIcon(targetData.icon)" :alt="sourceData.name">
                                    <div class="card-body">
                                        <h5 class="card-title">{{ targetData.name }}</h5>
                                        <h6 class="card-subtitle text-muted">{{ targetData.category }}</h6>
                                    </div>
                                    <div class="card-body" style="padding-top: 0;">
                                        <p class="card-text">
                                            <pre style="font-family: inherit; white-space: pre-wrap; overflow-x: hidden;" v-html="parseDesc(targetData.description)"></pre>
                                        </p>
                                    </div>
                                </div>
                                <div v-else="">
                                    <!-- no data -->
                                    <div class="card-body">
                                        <p class="card-text">
                                            Looks like this item doesn't exist in this region!
                                        </p>
                                    </div>
                                </div>
                            </div>

                            <!-- No target region -->
                            <div v-else="">
                                <h3 class="card-header">Select a region!</h3>
                            </div>


                            <div class="card-footer text-muted">
                                <select class="form-control" v-model="targetRegionBind" style="width: 200px; margin: 0 auto;" @change="selectTarget">
                                    <option disabled="" value="_dummy">Target Region</option>
                                    <option v-for="r in $root.regions" v-bind:value="r.region">
                                        {{ r.region }} ({{ r.version }})
                                    </option>
                                </select>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>


<script>
    import axios from "axios";

    export default {
        data: function () {
            return {
                queryId: this.$route.query.id,
                region: this.$route.query.region,
                invalid: false,
                loading: true,
                sourceRegion: null,
                targetRegionBind: "_dummy",
                targetRegion: null,
                sourceData: null,
                targetData: null,
                favorited: null
            }
        },
        created: function () {
            if (!this.queryId || !this.region) {
                this.invalid = true;
                this.loading = false;
                return;
            }

            // invalid region
            var region = this.$root.regions.find(r => r.region === this.region);
            if (!region) {
                this.invalid = true;
                this.loading = false;
                return;
            }

            this.sourceRegion = region;

            // get source data
            this.lookupInRegion(this.region)
                .then(d => this.sourceData = d)
                .then(() => {
                    // check for target
                    var preTarget = this.$route.query.target || window.localStorage.getItem("targetRegion");
                    if (preTarget && this.$root.regions.find(r => r.region === preTarget)) { // check for validity first
                        this.targetRegionBind = preTarget;
                        this.selectTarget();
                    }
                    return;
                })
                .then(() => this.loading = false);

            this.favorited = this.$root.favorites.find(f => f.id === this.queryId && f.region === this.region);
        },
        methods: {
            lookupInRegion: function (regionName) {
                return axios.get(`/api/item?id=${this.queryId}&region=${regionName}`)
                    .then(resp => {
                        if (resp.data.error && resp.data.error._errid === "NO_ITEM")
                            return null;

                        return resp.data.item;
                    });
            },
            selectTarget: function () {
                if (this.targetRegionBind === "_dummy")
                    return;
                this.targetRegion = this.$root.regions.find(r => r.region === this.targetRegionBind);
                window.localStorage.setItem("targetRegion", this.targetRegionBind);

                this.loading = true;
                this.lookupInRegion(this.targetRegionBind)
                    .then(d => this.targetData = d)
                    .then(() => this.loading = false)
                    .then(() => this.$router.replace({ path: "/search/lookup", query: { id: this.queryId, region: this.region, target: this.targetRegionBind } }));
            },
            favorite: function (toStatus) {
                if (toStatus) {
                    this.favorited = { id: this.queryId, name: this.sourceData.name, category: this.sourceData.category, region: this.region, icon: this.sourceData.icon };
                    this.$root.favorites.push(this.favorited);
                } else {
                    this.$root.favorites.splice(this.$root.favorites.indexOf(this.favorited), 1);
                    this.favorited = null;
                }

                this.$root.saveFavorites();
            },
            parseDesc: function (descArr) {
                let out = "";
                let orange = false;
                descArr.forEach(d => {
                    out += `<span${orange ? " style=\"color: orange;\"" : ""}>${this.$root.escapeHtml(d)}</span>`;
                    orange = !orange;
                });
                return out;
            }
        }
    }
</script>