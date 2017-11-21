<template>
    <div id="search">
        <h2>Search</h2>

        <div class="form-inline">
            <input class="form-control mr-sm-2" v-on:keyup.enter="search" type="text" placeholder="Search" v-model="query">
            <select class="form-control mr-sm-2" v-model="region">
                <option v-for="r in $root.regions" v-bind:value="r.region">
                    {{ r.region }} ({{ r.version }})
                </option>
            </select>
            <button class="btn btn-secondary my-2 my-sm-0" type="button" v-on:click="search">Search</button>
        </div>

        <div v-if="searching" class="text-center mt-2">
            <i class="fa fa-spinner fa-pulse fa-3x fa-fw"></i>
            <span class="sr-only">Loading...</span>
        </div>
        <div v-else="">
            <div v-if="lookup.length">
                <div class="alert alert-info mt-2">
                    <strong>Note!</strong> Click on an item in the table below to cross-region lookup!
                </div>
                <table class="table table-striped table-hover table-bordered">
                    <thead class="thead-dark">
                        <tr>
                            <th>ID #</th>
                            <th>Item</th>
                            <th>Category</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="item in lookup" v-on:click="redirLookup(item)">
                            <td>{{ item.id }}</td>
                            <td>{{ item.name }}</td>
                            <td>{{ item.category }}</td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div v-else-if="!emptyLoad" class="alert alert-danger mt-2">
                <strong>No items found.</strong> Try looking the item up perhaps in a different way!
            </div>
        </div>
    </div>
</template>


<script>
    import axios from "axios";

    export default {
        data: function () {
            return {
                query: window.MapleRIL.query,
                region: window.MapleRIL.region,
                lookup: [],
                emptyLoad: false,
                searching: false
            }
        },
        created: function () {
            if (!this.query || !this.region)
                this.emptyLoad = true;

            if (!this.region)
                this.region = this.$root.regions[0].region;

            this.search();
        },
        methods: {
            search: function (e) {
                if (!this.query || !this.region)
                    return;

                this.searching = true;
                axios.get(`/api/search?q=${this.query}&region=${this.region}`)
                    .then(resp => {
                        this.lookup = resp.data.items;
                        this.emptyLoad = false;
                        this.searching = false;

                        if (window.history.pushState)
                            window.history.pushState({}, "MapleRIL", `?q=${this.query}&region=${this.region}`); // update browser url
                    })
                    .catch(e => this.searching = false);
            },
            redirLookup: function (item) {
                window.location.href = `/lookup?id=${item.id}&region=${this.region}`;
            }
        }
    }
</script>