<template>
  <div class="video-container">

    <!-- Play button | playing toggles(switching) => functionality | binding class for hiding play button on play => depends on playing state -->
    <div class="play-button" :class="{'hide': playing}" @click.stop="playing = !playing">
      <v-icon size="78">mdi-play</v-icon>
    </div>

    <!-- :src is calling our /api/videos/{video} controller to open and read content from it | video is prop -->
    <!-- ref => this element becomes javascript object -->
    <!-- :poster is used for displaying thumbnails instead of preloading every video on the page -->
    <!-- preload="none" prevents preloading when we go to tricks submissions page, when we click, only then is loaded -->
    <video
      ref="video"
      width="400"
      muted loop
      :src="video"
      :poster="thumb"
      preload="none"
      playsinline
    >
    </video>
  </div>
</template>

<script>
  export default {
    // Component name
    name: "video-player",

    // Props
    props: {
      video: {
        required: true, // Required
        type: String, // Video is type of String
      },

      thumb: {
        required: true, // Required
        type: String, // Video is type of String
      }
    },

    // Component local state
    data: () => ({
      playing: false
    }),

    // Watcher for playing component data
    watch: {
      playing: function (newPlayingValue) {
        // If we are playing video |> newPlayingValue === true
        if (newPlayingValue) {
          // Vue.js functionality for grabbing all refs | play the video | play built in dom method
          this.$refs.video.play()
        } else {
          // Pause the video | pause built in dom method
          this.$refs.video.pause()
        }
      }
    }

  }
</script>

<style lang="scss" scoped>
  .video-container {
    display: flex;
    position: relative;
    width: 100%;

    /* Round top borders for video container */
    border-top-left-radius: inherit;
    border-top-right-radius: inherit;

    /* Targeting play-button */
    /* z-index => An element with greater stack order is always in front of an element with a lower stack order. */
    .play-button {
      position: absolute;
      display: flex;
      width: 100%;
      height: 100%;
      background-color: rgba(0,0,0,0.36);
      justify-content: center;
      align-items: center;
      z-index: 2; // Play button first, then video

      /* Class that gonna be present on .play-button as-well | Hides play button */
      &.hide {
        opacity: 0;
      }
    }

    /* Targeting video => ref => becomes js object */
    video {
      width: 100%;
      z-index: 1; // Play button first, then video

      /* Round top borders for video */
      border-top-left-radius: inherit;
      border-top-right-radius: inherit;
    }
  }
</style>
