<template>
  <!-- Stepper component - Steps -->
  <v-stepper v-model="step">
    <v-stepper-header>
      <v-stepper-step :complete="step > 1" step="1">Technique Information</v-stepper-step>

      <v-divider></v-divider>

      <v-stepper-step step="2">Review</v-stepper-step>
    </v-stepper-header>

    <v-stepper-items>
      <v-stepper-content step="1">
        <div>
          <!--? Step 1, Vuetify component that asks for technique name, description, stores it, on click goes to next step -->
          <v-text-field label="Name" v-model="form.name"></v-text-field>
          <v-text-field label="Description" v-model="form.description"></v-text-field>

          <v-select :items="categoryItems" v-model="form.category" label="Category"></v-select>
          <v-select :items="subcategoryItems" v-model="form.subCategory" label="Sub Category"></v-select>

          <!-- Chips are nice way to display multiple items from dropdown -->
          <v-select :items="techniqueItems" v-model="form.setUpAttacks" label="Set Up Attacks" multiple small-chips chips
                    deletable-chips></v-select>
          <v-select :items="techniqueItems" v-model="form.followUpAttacks" label="Follow Up Attacks" multiple small-chips chips
                    deletable-chips></v-select>
          <v-select :items="techniqueItems" v-model="form.counters" label="Counters" multiple small-chips chips
                    deletable-chips></v-select>


          <v-btn @click="step++">Next</v-btn>
        </div>
      </v-stepper-content>

      <v-stepper-content step="2">
        <!-- Button - Final step (2), Saving trick -->
        <div>
          <v-btn @click="save">Save</v-btn>
        </div>
      </v-stepper-content>
    </v-stepper-items>

  </v-stepper>
</template>

<script>
  import {mapGetters, mapState, mapActions, mapMutations} from "vuex";

  // Initial local state of component
  const initState = () => ({
    step: 1,
    form: {
      name: "",
      description: "",
      category: "",
      subCategory: "",
      setUpAttacks: [],
      followUpAttacks: [],
      counters: [],
    },
    testData: [
      {text: "Foo", value: 1},
      {text: "Bar", value: 2},
      {text: "Baz", value: 3},
    ]
  });

  // Component that is responsible for creating/saving technique
  export default {
    // Component name
    name: "techniques-steps",

    // Data is referencing initState function which holds local state of component -> this.$data
    data: initState,

    computed: {
      ...mapState("video-upload", ["active"]),
      ...mapGetters("techniques", ["techniqueItems", "categoryItems", "subcategoryItems"]),
    },

    // TODO EXPLORE WATCH
    // Watcher for active state prop
    watch: {
      "active": function (newValue) {
        // if newValue is false
        if (!newValue) {
          // Resets state to initial state
          Object.assign(this.$data, initState());
        }
      }
    },

    // Mapping modules mutation and action functions
    methods: {
      // Map mutations for video-upload module
      ...mapMutations("video-upload", ["reset"]),
      // Map actions for technique module
      ...mapActions("techniques", ["createTechnique"]),

      // Saving technique | #2
      async save() {
        // Creating new obj with data from our local form(state), binding local form to form
        // Local form is gonna get passed to store as payload
        await this.createTechnique({form: this.form});

        // Resets video-upload store, after saving, closing dialog
        // this refers to VueComponent, $data is VuesComponent local state, resetting component state
        this.reset();
        Object.assign(this.$data, initState());
      },
    },
  }
</script>

<style scoped>

</style>
