<template>
  <v-list>
    <v-list-item @click="$router.push(`/subcategory/${subcategory.id}`)" :key="`moderation-category-${subcategory.id}`"
                 v-for="subcategory in lists.subcategories">
      <v-list-item-content>
        <v-list-item-title>{{ subcategory.name }}</v-list-item-title>
        <v-list-item-subtitle>{{ subcategory.description }}</v-list-item-subtitle>
      </v-list-item-content>
      <v-list-item-content>
        <v-list-item-title>Last Updated</v-list-item-title>
        <v-list-item-subtitle>{{ subcategory.updated }}</v-list-item-subtitle>
      </v-list-item-content>
      <v-list-item-action>
        <v-btn icon @click.stop="">
          <v-icon>mdi-pencil</v-icon>
        </v-btn>
      </v-list-item-action>
      <v-list-item-action>
        <v-btn icon @click.stop="">
          <v-icon>mdi-delete</v-icon>
        </v-btn>
      </v-list-item-action>
    </v-list-item>
  </v-list>
</template>

<script>
import {mapState} from "vuex";

export default {
  name: "moderation-subcategory-overview",
  computed: mapState('techniques', ['lists'])
}
</script>

<style scoped>

</style>
