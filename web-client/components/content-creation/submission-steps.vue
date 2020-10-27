<template>
  <!-- Card -->
  <v-card>

    <!-- Title -->
    <v-card-title>
      Create Submission
      <v-spacer></v-spacer>

      <!-- Button - X -->
      <!-- On click call close method which is mixin -->
      <v-btn icon @click="close">
        <v-icon>mdi-close</v-icon>
      </v-btn>
    </v-card-title>

    <!-- Stepper component -->
    <v-stepper class="rounded-0" v-model="step">
      <v-stepper-header class="elevation-0">
        <v-stepper-step :complete="step > 1" step="1">Upload Video</v-stepper-step>

        <v-divider></v-divider>

        <v-stepper-step :complete="step > 2" step="2">Select Technique</v-stepper-step>

        <v-divider></v-divider>

        <v-stepper-step :complete="step > 3" step="3">Submission</v-stepper-step>

        <v-divider></v-divider>

        <v-stepper-step step="4">Review</v-stepper-step>
      </v-stepper-header>

      <!-- Form input -->
      <v-stepper-items class="fpt-0">
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
            <v-select :items="lists.techniques.map(t => ({value: t.slug, text: t.name}))" v-model="form.techniqueId"
                      label="Select Technique"></v-select>

            <!-- Button -->
            <div class="d-flex justify-center">
              <v-btn @click="step++">Next</v-btn>
            </div>
          </div>
        </v-stepper-content>

        <v-stepper-content step="3">
          <div>
            <!-- Step 3, Vuetify component for saving submission, stores it, on click goes to next step -->
            <v-text-field label="Description" v-model="form.description"></v-text-field>

            <!-- Button -->
            <div class="d-flex justify-center">
              <v-btn @click="step++">Next</v-btn>
            </div>
          </div>
        </v-stepper-content>

        <v-stepper-content step="4">
          <!-- Button - Final step (4), Saving submission for particular technique -->
          <div class="d-flex justify-center">
            <v-btn @click="save">Save</v-btn>
          </div>
        </v-stepper-content>
      </v-stepper-items>
    </v-stepper>
  </v-card>
</template>

<script>
import {mapActions, mapMutations, mapState} from "vuex";
import {close, form} from "@/components/content-creation/_shared";

export default {
  // Component name
  name: "submission-steps",

  mixins: [
    close,

    // This is the form that we are gonna be appending in case of upload
    form(() => ({
      techniqueId: "",
      video: "",
      description: ""
    }))
  ],

  // Data is referencing initState function which holds local state of component -> this.$data
  data: () => ({
    step: 1,
  }),

  computed: mapState("techniques", ["lists"]),

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
