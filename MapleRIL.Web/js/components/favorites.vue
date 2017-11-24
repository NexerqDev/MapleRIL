<template>
    <div id="favorites">
        <h1>Favorites</h1>

        <div v-if="$root.favorites.length">
            <table class="table table-striped table-hover table-bordered">
                <thead class="thead-dark">
                    <tr>
                        <th width="32px"></th>
                        <th>Region</th>
                        <th>ID #</th>
                        <th>Item</th>
                        <th>Category</th>
                        <th><i class="fa fa-star-o" aria-hidden="true"></i></th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="fav in $root.favorites" v-on:click="gotoFav(fav)">
                        <td><img v-bind:src="fav.icon" width="32px"></td>
                        <td>{{ fav.region }}</td>
                        <td>{{ fav.id }}</td>
                        <td>{{ fav.name }}</td>
                        <td>{{ fav.category }}</td>
                        <td>
                            <i class="fa fa-star" aria-hidden="true" style="cursor: pointer;" v-on:click.stop="removeFav(fav)"></i>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div v-else>
            <div class="alert alert-warning">
              <h4>No favorites found.</h4>
              <p><strong>No favorites could be found!</strong> On an item lookup, click the <i class="fa fa-star-o" aria-hidden="true"></i> icon to save it to your favorites!</p>
            </div>
        </div>
    </div>
</template>

<script>
    export default {
        methods: {
            gotoFav: function (fav) {
                this.$router.push({ path: "/search/lookup", query: { id: fav.id, region: fav.region }});
            },
            removeFav: function (fav) {
                this.$root.favorites.splice(this.$root.favorites.indexOf(fav), 1);
                this.$root.saveFavorites();
            }
        }
    }
</script>