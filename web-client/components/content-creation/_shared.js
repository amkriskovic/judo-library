// * Mixin for shared functionality * //

import {mapActions} from 'vuex';

export const close = {
  methods: {
    ...mapActions("video-upload", ["cancelUpload"]),

    // Wrapping up cancelUpload function into close func
    close() {
      this.cancelUpload();
    }
  }
}
