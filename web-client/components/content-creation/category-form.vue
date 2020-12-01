<template>
  <v-card>
    <v-card-title>
      Create Category

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
      </v-form>

    </v-card-text>

    <v-card-actions class="d-flex justify-center">
      <v-btn :disabled="!validation.valid" color="primary" @click="$refs.form.validate() ? save() : 0">Create</v-btn>
    </v-card-actions>
  </v-card>
</template>

<script>

  import {close} from "./_shared";

  export default {
    // Component name
    name: "category-form",

    data: () => ({
      form: {
        name: "",
        description: "",
      },
      validation: {
        valid: false,
        name: [v => !!v || "Name is required."],
        description: [v => !!v || "Description is required."]
      },
    }),

    mixins: [close],

    methods: {
      save() {
        // Making post request to our API controller with payload of form
        this.$axios.$post("/api/categories", this.form);

        // Reset component
        this.close();
      }
    },

  }
</script>

<style scoped>

</style>
