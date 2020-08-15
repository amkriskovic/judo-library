<template>
  <v-card>
    <v-card-title>
      Create Sub-Category

      <v-spacer></v-spacer>

      <!-- Button - X -->
      <!-- On click call close method which is mixin -->
      <v-btn icon @click="close">
        <v-icon>mdi-close</v-icon>
      </v-btn>
    </v-card-title>

    <v-card-text>
      <v-text-field label="Name" v-model="form.name"></v-text-field>
      <v-text-field label="Description" v-model="form.description"></v-text-field>
      <v-select label="Category" :items="categoryItems" v-model="form.categoryId"></v-select>
    </v-card-text>

    <v-card-actions class="d-flex justify-center">
      <v-btn @click="save">Save</v-btn>
    </v-card-actions>
  </v-card>
</template>

<script>
  import {mapGetters} from "vuex";
  import {close} from "./_shared";

  export default {
    // Component name
    name: "subcategory-form",

    data: () => ({
      form: {
        name: "",
        description: "",
        categoryId: ""
      }
    }),

    mixins: [close],

    computed: mapGetters("techniques", ["categoryItems"]),

    methods: {
      save() {
        // Making post request to our API controller with payload of form
        this.$axios.post("/api/subcategories", this.form);

        // Reset component
        this.close();
      }
    },

  }
</script>

<style scoped>

</style>
