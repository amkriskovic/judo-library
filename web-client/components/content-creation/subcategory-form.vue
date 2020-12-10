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

  mixins: [close],

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

  created() {
    if (this.editPayload) {
      const {id, name, description} = this.editPayload
      Object.assign(this.form, {id, name, description})
    }
  },

  methods: {
    save() {
      if (this.form.id) {
        this.$axios.put("/api/subcategories", this.form)
      } else {
        this.$axios.post("/api/subcategories", this.form)
      }

      // Reset component
      this.close();
    }
  },

  computed: {
    ...mapState('content-update', ['editPayload']),
    ...mapState("techniques", ["lists", "dictionary"]),
  }


}
</script>

<style scoped>

</style>
