// Root store function, with initial state
const initState = () => ({
  uploadPromise: null,
  active: false,
  component: null
})

// Export initState function as constant state
export const state = initState;

// Mutations <- write to state
export const mutations = {

  // Get's stepper based on action that we wanna perform, create technique or upload submission, passing component as payload
  // Which will be activated once we select it from menu/dropdown
  activate(state, {component}) {
    state.active = true;
    state.component = component;
  },

  // Hide in order to prevent nullifying state (uploadPromise) when creating submission
  hide(state) {
    // Closing <- state is gonna be inactive <- dropping video-uploads down
    state.active = false;
  },

  // Setting uploadPromise to uploadPromise state
  setTask(state, {uploadPromise}) {
    // Assigning uploadPromise from payload to upload promise state
    state.uploadPromise = uploadPromise;
  },

  // Resets state to initial state
  reset(state) {
    Object.assign(state, initState())
  }

}

// When handling API calls (database), use actions
export const actions = {

  // Create technique, with payload <- form <- obj that is coming in
  async startVideoUpload({commit, dispatch}, {form}) {
    // Store the uploadPromise in variable after we make http post req to our api
    // Api will take care of saving video to our storage
    // We are getting promise with some state, * for data we need to await it
    //# $post grabs data: object from full response
    const uploadPromise = this.$axios.$post("/api/videos", form);

    // Commit uploadPromise Promise to state <- store it in state
    commit("setTask", {uploadPromise});
  },

  // Create submission with accessing state of store, and with payload <- form
  async createSubmission({state, commit, dispatch}, {form}) {
    if (!state.uploadPromise) {
      console.log("uploadPromise is null")
      return;
    }

    // Assign upload promise (which we resolve here -> getting data) to forms video prop
    form.video = await state.uploadPromise;

    // Dispatching createSubmission action with payload form, where we have our form data
    await dispatch("submissions/createSubmission", {form}, {root: true});

    // After creating submission, flush(reset) component to initial state
    commit("reset");
  }

}

