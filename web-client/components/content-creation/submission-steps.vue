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

        <v-stepper-step step="3">Review</v-stepper-step>
      </v-stepper-header>

      <!-- Form input -->
      <v-stepper-items class="fpt-0">
        <v-stepper-content step="1">
          <div>
            <v-file-input
              v-model="file"
              accept="video/*"
              @change="handleFile">
            </v-file-input>
          </div>
        </v-stepper-content>

        <v-stepper-content step="2">
          <v-form ref="form" v-model="validation.valid">
            <v-autocomplete
              :items="lists.techniques.map(t => ({value: t.slug, text: t.name}))"
              :rules="validation.techniqueId"
              v-model="form.techniqueId"
              label="Select Technique">
            </v-autocomplete>

            <v-text-field
              label="Description"
              :rules="validation.description"
              v-model="form.description">
            </v-text-field>


            <!-- Button -->
            <div class="d-flex justify-center">
              <v-btn :disabled="!validation.valid" @click="$refs.form.validate() ? step++ : 0">Next</v-btn>
            </div>
          </v-form>
        </v-stepper-content>

        <v-stepper-content step="3">
          <div><strong>Filename:</strong> {{fileName}}</div>
          <div v-if="form.techniqueId"><strong>Technique:</strong> {{ dictionary.techniques[form.techniqueId].name }}</div>
          <div><strong>Description:</strong> {{ form.description }}</div>


          <!-- Button - Final step (4), Saving submission for particular technique -->
          <div class="d-flex mt-3">
            <v-btn @click="restart">Restart</v-btn>
            <v-btn class="mx-2" @click="step--">Edit</v-btn>

            <v-spacer/>

            <v-btn color="primary" @click="save">Complete</v-btn>
          </div>
        </v-stepper-content>
      </v-stepper-items>
    </v-stepper>
  </v-card>
</template>

<script>
import {mapActions, mapMutations, mapState} from "vuex";
import {close, form} from "@/components/content-creation/_shared";

const initForm = () => ({
  techniqueId: "",
  video: "",
  description: ""
})

export default {
  // Component name
  name: "submission-steps",

  mixins: [
    close,

    // This is the form that we are gonna be appending in case of upload
    form(initForm),
  ],

  // Data is referencing initState function which holds local state of component -> this.$data
  data: () => ({
    step: 1,
    file: null,
    validation: {
      valid: false,
      techniqueId: [v => !!v || "Technique is required."],
      description: [v => !!v || "Description is required."],
    }
  }),

  computed: {
    ...mapState("techniques", ["lists", "dictionary"]),
    fileName() {
      return this.file ? this.file.name : ""
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
      this.createSubmission({form: this.form})

      // Hides whatever stepper(component) was active <- dropping
      this.hide();
    },

    restart() {
      this.form = initForm()
      this.cancelUpload({hard: false})
      this.step = 1
      this.file = null
    },

  }

}
</script>

<style scoped>

</style>
