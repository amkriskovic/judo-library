﻿// Root store function, with initial state
const initState = (active = false, component = null) => ({
  uploadPromise: null,
  uploadCancelSource: null,
  uploadCompleted: false,
  active: active,
  component: component,
  editPayload: null,
  setup: null
})

// Export initState function as constant state
export const state = initState;

// Mutations <- write to state
export const mutations = {

  // Get's stepper based on action that we wanna perform, create technique or upload submission, passing component as payload
  // Which will be activated once we select it from menu/dropdown
  activate(state, {component, editPayload = null, setup = null}) {
    state.active = true;
    state.component = component;

    state.editPayload = editPayload;

    state.setup = setup
  },

  // Hide in order to prevent nullifying state (uploadPromise) when creating submission
  hide(state) {
    // Closing <- state is gonna be inactive <- dropping content-creations down
    state.active = false;
  },

  // Setting uploadPromise & uploadCancelSource to state
  setTask(state, {uploadPromise, source}) {
    // Assigning uploadPromise from payload to upload promise state
    state.uploadPromise = uploadPromise;

    // Assigning source from payload to uploadCancelSource state, passing cancellation token
    state.uploadCancelSource = source;
  },

  // Setting state of uploadCompleted to true
  completeUpload(state) {
    state.uploadCompleted = true;
  },

  // Resets state to initial state
  reset(state, {hard}) {
    if (hard) {
      Object.assign(state, initState())
    } else {
      Object.assign(state, initState(true, state.component))
    }
  }

}

// When handling API calls (database), use actions
export const actions = {

  // Create technique, with payload <- form <- obj that is coming in
  startVideoUpload({commit, dispatch}, {form}) {

    // * Grab the cancellation token source from axios cancel token when starting video upload
    const source = this.$axios.CancelToken.source();

    // * $post grabs data: object from full response
    // Store the uploadPromise in variable after we make http post req to our api
    // Api will take care of saving video to our storage
    // We are getting promise with some state, * for data we need to await it
    // Extract the data from response
    // Generate cancelToken from the source
    const uploadPromise = this.$axios.$post("/api/files", form, {

      // Supply an option/s -> progress -> false -> removes loading bar (spinning)
      progress: false,
      cancelToken: source.token
    })
      .then(video => {
        // Call mutation completeUpload which will mark video -> upload complete as true
        commit("completeUpload");

        // Return video URL which will get stored to the uploadPromise, data is actual TEMP video name
        return video;
      })
      .catch(err => {
        // If err is cancel -> cancellation request
        if (this.$axios.isCancel(err)) {
          // todo: popup notify
        }
      });

    // Commit uploadPromise Promise to state <- store it in state & source which is cancellation token
    commit("setTask", {uploadPromise, source});
  },

  // Action for canceling upload, cancelling request / deleting video
  async cancelUpload({state, commit}, {hard}) {
    // If we have upload promise
    if (state.uploadPromise) {
      // If upload was completed -> temp video has been uploaded/stored in working dir
      if (state.uploadCompleted) {
        // Hide -> drop the form
        commit("hide");

        // Grab the "temporary" video from upload promise
        const video = await state.uploadPromise;

        console.log('cancelUpload->video: ', video);


        // Call API controller in order to delete particular video
        await this.$axios.delete("/api/files/" + video);
      } else {
        // If upload wasn't completed (cancelled while pending), cancel the source
        console.log('uploadCancelSource.cancel() CLOSE');
        state.uploadCancelSource.cancel();
      }
    }

    // Reset the state
    commit("reset", {hard});
  },

  // Create submission with accessing state of store, and with payload <- form
  async createSubmission({state, commit, dispatch}, {form}) {
    if (!state.uploadPromise) {
      console.log("uploadPromise is null")
      return;
    }

    // Assign upload promise (which we resolve here -> getting data) to forms video prop
    form.video = await state.uploadPromise;

    await this.$axios.$post("/api/submissions", form);

    // After creating submission, flush(reset) component to initial state
    commit("reset", {hard: true});
  }

}

