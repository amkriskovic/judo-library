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
          <div>
            <!--? Step 1, Vuetify component that asks for technique name, description, stores it, on click goes to next step -->
            <v-text-field label="Name" v-model="form.name"></v-text-field>
            <v-text-field label="Description" v-model="form.description"></v-text-field>

            <!-- Iterating over arr of categories, and mapping each to new obj. with 2 props => value and text -->
            <!-- Value is gonna be category Id, and text is gonna be category Name -->
            <v-select :items="lists.categories.map(c => ({value: c.id, text: c.name}))"
                      v-model="form.category"
                      label="Category">
            </v-select>

            <v-select :items="lists.subcategories.map(sc => ({value: sc.id, text: sc.name}))"
                      v-model="form.subCategory"
                      label="Sub Category">
            </v-select>

            <!-- Chips are nice way to display multiple items from dropdown -->
            <!-- Filter only if we dont have Id on the form, means we are creating || if technique setup attack is not equal to form id, -->
            <!-- means we cant select the one we edit as setUpAttack, filters out everything else  -->
            <v-select :items="lists.techniques.filter(tsa => !form.id || tsa.id !== form.id).map(tsa => ({value: tsa.id, text: tsa.name}))"
                      v-model="form.setUpAttacks"
                      label="Set Up Attacks"
                      multiple small-chips
                      chips deletable-chips>
            </v-select>

            <v-select :items="lists.techniques.filter(tfa => !form.id || tfa.id !== form.id).map(tfa => ({value: tfa.id, text: tfa.name}))"
                      v-model="form.followUpAttacks"
                      label="Follow Up Attacks"
                      multiple
                      small-chips chips deletable-chips>
            </v-select>

            <v-select :items="lists.techniques.filter(tc => !form.id || tc.id !== form.id).map(tc => ({value: tc.id, text: tc.name}))"
                      v-model="form.counters"
                      label="Counters"
                      multiple small-chips chips
                      deletable-chips>
            </v-select>


            <div class="d-flex justify-center">
              <v-btn @click="step++">Next</v-btn>
            </div>
          </div>
        </v-stepper-content>

        <v-stepper-content step="2">
          <v-text-field v-if="editing" label="Reason For Change" v-model="form.reason"></v-text-field>

          <!-- Button - Final step (2), Saving trick -->
          <div class="d-flex justify-center">
            <v-btn :disabled="editing && form.reason.length <= 5" @click="save">Save</v-btn>
          </div>
        </v-stepper-content>
      </v-stepper-items>

    </v-stepper>
  </v-card>

</template>

<script>
import {mapActions, mapState} from "vuex";
  import {close} from "./_shared";

  // Component that is responsible for creating/saving technique
  export default {
    // Component name
    name: "techniques-steps",

    // Data is referencing initState function which holds local state of component -> this.$data
    data: () => ({
      step: 1,
      form: {
        name: "",
        description: "",
        category: "",
        subCategory: "",
        reason: "",
        setUpAttacks: [],
        followUpAttacks: [],
        counters: [],
      },
      testData: [
        {text: "Foo", value: 1},
        {text: "Bar", value: 2},
        {text: "Baz", value: 3},
      ]
    }),

    mixins: [close],

    computed: {
      ...mapState("techniques", ["lists"]),
      ...mapState("video-upload", ["editing", "editPayload"])
    },

    // When this component get's created => grab the editingPayload and stick it on the form
    created() {
      // If editing is true
      if (this.editing) {
        // Assign editPayload to existing form | target, source
        // Useful because it will pre-populate fields that where filled
        Object.assign(this.form, this.editPayload)
      }
    },

    // Mapping modules mutation and action functions
    methods: {
      // Map actions for technique module
      ...mapActions("techniques", ["createTechnique", "updateTechnique"]),

      // Saving technique | #2
      async save() {
        // If we are editing
        if (this.editing) {
          // Make an update -> PUT req.
          await this.updateTechnique({form: this.form});
        } else {
          // Creating new obj with data from our local form(state), binding local form to form
          // Local form is gonna get passed to store as payload
          await this.createTechnique({form: this.form});
        }

        // * this refers to VueComponent, close will set component to null and hide it, eventually -> pipeline
        this.close();
      },
    },
  }
</script>

<style scoped>

</style>
