<template>
  <v-card>
    <v-card-title>Create Sub-Category</v-card-title>

    <v-card-text>
      <v-text-field label="Name" v-model="form.name"></v-text-field>
      <v-text-field label="Description" v-model="form.description"></v-text-field>
      <v-select label="Category" :items="categoryItems" v-model="form.categoryId"></v-select>
    </v-card-text>

    <v-card-actions>
      <v-btn @click="save">Save</v-btn>
    </v-card-actions>
  </v-card>
</template>

<script>
  import {mapGetters} from "vuex";

  const initState = () => ({
    form: {
      name: "",
      description: "",
      categoryId: ""
    }
  });

  export default {
    // Component name
    name: "subcategory-form",

    data: initState,

    computed: mapGetters("techniques", ["categoryItems"]),

    methods: {
      save() {
        // Making post request to our API controller with payload of form
        this.$axios.post("/api/subcategories", this.form);
      }
    },

  }
</script>

<style scoped>

</style>
