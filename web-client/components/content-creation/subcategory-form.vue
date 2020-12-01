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
      <v-form ref="form" v-model="validation.valid">
        <v-text-field :rules="validation.name" label="Name" v-model="form.name"></v-text-field>
        <v-text-field :rules="validation.description" label="Description" v-model="form.description"></v-text-field>

        <v-select
          :rules="validation.categoryId"
          :items="lists.categories.map(c => ({value: c.id, text: c.name}))"
          v-model="form.categoryId"
          label="Category">
        </v-select>
      </v-form>

    </v-card-text>

    <v-card-actions class="d-flex justify-center">
      <v-btn :disabled="!validation.valid" color="primary" @click="$refs.form.validate() ? save() : 0">Create</v-btn>
    </v-card-actions>
  </v-card>
</template>

<script>
import {mapState} from "vuex";
import {close} from "./_shared";

export default {
  // Component name
  name: "subcategory-form",

  data: () => ({
    form: {
      name: "",
      description: "",
      categoryId: ""
    },
    validation: {
      valid: false,
      name: [v => !!v || "Name is required."],
      description: [v => !!v || "Description is required."],
      categoryId: [v => !!v || "Category is required."]
    },
  }),

  mixins: [close],

  computed: mapState("techniques", ["lists", "dictionary"]),

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
