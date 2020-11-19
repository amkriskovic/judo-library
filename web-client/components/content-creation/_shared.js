// * Mixin for shared functionality * //

import {mapActions, mapState} from 'vuex';

export const close = {
  methods: {
    ...mapActions("video-upload", ["cancelUpload"]),

    // Wrapping up cancelUpload function into close func
    close() {
      this.cancelUpload();
    }
  }
}


export const form = (formFactory) => ({
  data: () => ({
    // Execute formFactory and assign it to form prop in form function => acts like generic
    form: formFactory()
  }),

  created: function () {
    // Call this setup which comes from video-upload, with this form that's generated with formFactory
    if (this.setup) {
      this.setup(this.form)
    }
  },

  computed: {
    ...mapState("video-upload", ["setup"])
  }
})

