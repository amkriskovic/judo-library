<template>
  <!-- Card -->
  <v-card>

    <!-- Title -->
    <v-card-title>
      Create Technique
      <v-spacer></v-spacer>

      <!-- Button - X -->
      <!-- On click call close method which is mixin -->
      <v-btn icon @click="close">
        <v-icon>mdi-close</v-icon>
      </v-btn>
    </v-card-title>

    <!-- Stepper component - Steps -->
    <v-stepper class="rounded-0" v-model="step">
      <v-stepper-header class="elevation-0">
        <v-stepper-step :complete="step > 1" step="1">Technique Information</v-stepper-step>

        <v-divider></v-divider>

        <v-stepper-step step="2">Review</v-stepper-step>
      </v-stepper-header>

      <!-- Form inputs -->
      <v-stepper-items class="fpt-0">
        <v-stepper-content step="1">
          <v-form ref="form" v-model="validation.valid">
            <v-text-field :rules="validation.name" label="Name" :disabled="!!editPayload"
                          v-model="form.name"></v-text-field>

            <v-text-field :rules="validation.description" label="Description" v-model="form.description"></v-text-field>

            <v-select :rules="validation.category" :items="lists.categories.map(c => ({value: c.id, text: c.name}))"
                      v-model="form.category" label="Category"></v-select>

            <v-select :rules="validation.subCategory"
                      :items="lists.subcategories.map(sc => ({value: sc.id, text: sc.name}))" v-model="form.subCategory"
                      label="Sub Category"></v-select>

            <v-autocomplete
              :items="lists.techniques.filter(tsa => !form.id || tsa.id !== form.id).map(tsa => ({value: tsa.id, text: tsa.name}))"
              v-model="form.setUpAttacks" label="Set Up Attacks" multiple small-chips chips
              deletable-chips></v-autocomplete>

            <v-autocomplete
              :items="lists.techniques.filter(tfa => !form.id || tfa.id !== form.id).map(tfa => ({value: tfa.id, text: tfa.name}))"
              v-model="form.followUpAttacks" label="Follow Up Attacks" multiple small-chips chips
              deletable-chips></v-autocomplete>

            <v-autocomplete
              :items="lists.techniques.filter(tc => !form.id || tc.id !== form.id).map(tc => ({value: tc.id, text: tc.name}))"
              v-model="form.counters" label="Counters" multiple small-chips chips deletable-chips></v-autocomplete>


            <div class="d-flex justify-center">
              <v-btn :disabled="!validation.valid" @click="$refs.form.validate() ? step++ : 0">Next</v-btn>
            </div>
          </v-form>
        </v-stepper-content>

        <v-stepper-content step="2">

          <div><strong>Name:</strong> {{ form.name }}</div>
          <div><strong>Description:</strong> {{ form.description }}</div>
          <div v-if="form.category"><strong>Category:</strong> {{ dictionary.categories[form.category].name }}</div>
          <div v-if="form.subCategory"><strong>Sub-Category:</strong>
            {{ dictionary.subcategories[form.subCategory].name }}
          </div>

          <div><strong>Set Up Attacks:</strong> {{
              form.setUpAttacks.map(x => dictionary.techniques[x].name).join(', ')
            }}
          </div>
          <div><strong>Follow Up Attacks:</strong>
            {{ form.followUpAttacks.map(x => dictionary.techniques[x].name).join(', ') }}
          </div>
          <div><strong>Counters:</strong> {{ form.counters.map(x => dictionary.techniques[x].name).join(', ') }}</div>

          <v-text-field v-if="requireReason" label="Reason For Change" v-model="form.reason"></v-text-field>

          <!-- Button - Final step (2), Saving trick -->
          <div class="d-flex mt-3">
            <v-btn @click="step--">Edit</v-btn>
            <v-spacer/>
            <v-btn color="primary" :disabled="requireReason && form.reason.length <= 5" @click="save">
              {{ !!editPayload ? "Update" : "Create" }}
            </v-btn>
          </div>
        </v-stepper-content>
      </v-stepper-items>

    </v-stepper>
  </v-card>

</template>

<script>
import {mapActions, mapState} from "vuex";
import {close, form} from "@/components/content-creation/_shared";
import {VERSION_STATE} from "@/components/moderation";

// Component that is responsible for creating/saving technique
export default {
  // Component name
  name: "techniques-steps",

  mixins: [close, form(() => ({
    name: "",
    description: "",
    category: "",
    subCategory: "",
    reason: "",
    setUpAttacks: [],
    followUpAttacks: [],
    counters: [],
  }))],

  // Data is referencing initState function which holds local state of component -> this.$data
  data: () => ({
    step: 1,
    validation: {
      valid: false,
      name: [v => !!v || "Name is required."],
      description: [v => !!v || "Description is required."],
      category: [v => !!v || "Category is required."],
      // v => v.length > 0
      subCategory: [v => !!v || "Sub-Category is required."],
    },
    testData: [
      {text: "Foo", value: 1},
      {text: "Bar", value: 2},
      {text: "Baz", value: 3},
    ]
  }),

  computed: {
    ...mapState("library", ["lists", "dictionary"]),
    ...mapState("content-creation", ["editPayload"]),
    staged() {
      return this.form.state === VERSION_STATE.STAGED
    },
    requireReason() {
      return this.editPayload && !this.staged
    }
  },

  // When this component get's created => grab the editingPayload and stick it on the form
  created() {
    // If editing is true
    if (this.editPayload) {
      // Assign editPayload to existing form | target, source
      // Useful because it will pre-populate fields that where filled
      Object.assign(this.form, this.editPayload)
    }
  },

  // Mapping modules mutation and action functions
  methods: {
    async save() {
      if (this.form.id) {
        if (this.staged) {
          await this.$axios.$put("/api/techniques/staged", this.form)
        } else {
          await this.$axios.$put("/api/techniques", this.form)
        }
      } else {
        await this.$axios.$post("/api/techniques", this.form)
      }

      this.broadcastUpdate()

      // * this refers to VueComponent, close will set component to null and hide it, eventually -> pipeline
      this.close();
    },
  },
}
</script>

<style scoped>

</style>
