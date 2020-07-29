<template>
  <!-- Stepper component -->
  <v-stepper v-model="step">
    <v-stepper-header>
      <v-stepper-step :complete="step > 1" step="1">Upload Video</v-stepper-step>

      <v-divider></v-divider>

      <v-stepper-step :complete="step > 2" step="2">Select Technique</v-stepper-step>

      <v-divider></v-divider>

      <v-stepper-step :complete="step > 3" step="3">Submission</v-stepper-step>

      <v-divider></v-divider>

      <v-stepper-step step="4">Review</v-stepper-step>
    </v-stepper-header>

    <v-stepper-items>
      <v-stepper-content step="1">
        <div>
          <!-- Step 1, Vuetify component that accepts all types of videos, on change file upload process -->
          <v-file-input accept="video/*" @change="handleFile"></v-file-input>
        </div>
      </v-stepper-content>

      <v-stepper-content step="2">
        <div>
          <!-- # Dropdown for selecting technique -->
          <!-- Step 2, Vuetify component for selecting technique from dropdown, on click goes to next step -->
          <!-- Binds selected technique to techniqueId which lives in local form state -->
          <v-select :items="techniqueItems" v-model="form.techniqueId" label="Select Technique"></v-select>
          <v-btn @click="step++">Next</v-btn>
        </div>
      </v-stepper-content>

      <v-stepper-content step="3">
        <div>
          <!-- Step 3, Vuetify component for saving submission, stores it, on click goes to next step -->
          <v-text-field label="Description" v-model="form.description"></v-text-field>
          <v-btn @click="step++">Next</v-btn>
        </div>
      </v-stepper-content>

      <v-stepper-content step="4">
        <!-- Button - Final step (4), Saving submission for particular technique -->
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
      techniqueId: "",
      video: "",
      description: ""
    }
  });

  export default {
    // Component name
    name: "submission-steps",

    // Data is referencing initState function which holds local state of component -> this.$data
    data: initState,

    computed: {
      ...mapGetters("techniques", ["techniqueItems"]),
      ...mapState("video-upload", ["active"])
    },

    // Watcher for active state prop
    watch: {
      "active": function (newActiveValue) {
        // If newActiveValue is false
        if (!newActiveValue) {
          // Resets component local state to initial state
          Object.assign(this.$data, initState());
        }
      }
    },

    methods: {
      ...mapMutations("video-upload", ["hide"]),
      ...mapActions("video-upload", ["startVideoUpload", "createSubmission"]),

      // Handling file for upload <- saving video | #1
      async handleFile(file) {
        // If there is no file, return <- undefined
        if (!file) return;

        // Create dynamic form for step 1
        const form = new FormData();

        // Append to form name="video" and actual data, which is video file | name="video" value=file
        form.append("video", file);

        // Invoke video uploading after creating form, by passing that form(FormData) which contains only video information
        this.startVideoUpload({form});

        // Increment step by 1
        this.step++;
      },

      // Saving technique | #2
      save() {
        // Invoke createSubmission action with payload of form object, which is in our local component state
        this.createSubmission({form: this.form});

        // Hides whatever stepper(component) was active <- dropping
        this.hide();
      }

    }

  }
</script>

<style scoped>

</style>
