<template>
  <div>
    <v-list>
      <v-list-item @click="$router.push(`/subcategory/${subcategory.slug}`)"
                   :key="`moderation-subcategory-${subcategory.id}`"
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
          <v-btn icon @click.stop="edit(subcategory)">
            <v-icon>mdi-pencil</v-icon>
          </v-btn>
        </v-list-item-action>
        <v-list-item-action>
          <v-btn icon @click.stop="selectedSubCategory = subcategory">
            <v-icon>mdi-swap-horizontal</v-icon>
          </v-btn>
        </v-list-item-action>
      </v-list-item>
    </v-list>
    <v-dialog :value="selectedSubCategory" width="300" persistent>
      <v-card v-if="selectedSubCategory">
        <v-card-title>Migrate {{ selectedSubCategory.name }}?</v-card-title>
        <v-card-text>
          <v-select
            :items="lists.subcategories.filter(x => x.id !== selectedSubCategory.id).map(x => ({value: x.id, text: x.name}))"
            v-model="target"
            label="Sub Category"/>
        </v-card-text>
        <v-card-actions>
          <v-btn color="primary" @click="selectedSubCategory = null">no</v-btn>
          <v-spacer/>
          <v-btn :disabled="target === 0" @click="migrate">yes</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script>
import {mapMutations, mapState} from "vuex";
import SubCategoryForm from "@/components/content-creation/subcategory-form";
import {EVENTS} from "@/data/events";

export default {
  name: "moderation-subcategory-overview",
  data: () => ({
    selectedSubCategory: null,
    target: 0
  }),
  methods: {
    ...mapMutations('content-creation', ['activate']),
    edit(subcategory) {
      this.activate({
        component: SubCategoryForm,
        editPayload: subcategory,
        setup: null
      })
    },
    migrate() {
      return this.$axios.put(`/api/subcategories/${this.selectedSubCategory.id}/${this.target}`, null)
        .then(() => this.$nuxt.$emit(EVENTS.CONTENT_UPDATED))
        .finally(() => {
          this.selectedSubCategory = null
          this.target = 0
        })
    }
  },
  computed: mapState('library', ['lists'])
}
</script>

<style scoped>

</style>
