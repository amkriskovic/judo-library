<template>
  <v-card class="my-3">
    <!-- Injecting user-header component, providing username from submission -->
    <user-header :username="submission.user.username" :image-url="submission.user.image">
      <template v-slot:append>
        <span class="caption grey--text">{{submission.created}}</span>
      </template>
    </user-header>

    <v-card-text>{{ submission.description }}</v-card-text>

    <!-- Injecting video player component with dynamic binding of video where video is string -->
    <!-- * Getting specific videos from submissions store |> state => fetchSubmissionsForTechnique filling state -->
    <video-player :video="submission.video" :thumb="submission.thumb"/>

    <v-card-actions>
      <v-btn small icon :color="submission.vote === 1 ? 'blue' : ''" @click="vote(1)">
        <v-icon>mdi-chevron-up</v-icon>
      </v-btn>
      <span class="mx-2">{{submission.score}}</span>
      <v-btn small icon :color="submission.vote === -1 ? 'blue' : ''" @click="vote(-1)">
        <v-icon>mdi-chevron-down</v-icon>
      </v-btn>

      <v-spacer/>

      <v-btn icon @click="showComments = !showComments">
        <v-icon>mdi-comment</v-icon>
      </v-btn>
    </v-card-actions>

    <if-auth v-if="showComments" class="px-3 pb-2">
      <template v-slot:allowed>
        <comment-section :parent-id="submission.id" :parent-type="submissionParentType"/>
      </template>

      <template v-slot:forbidden="{login}">
        <div class="d-flex justify-center">
          <v-btn @click="login">Sign in to Comment</v-btn>
        </div>
      </template>
    </if-auth>

  </v-card>
</template>

<script>
import UserHeader from "@/components/user-header";
import VideoPlayer from "@/components/video-player";
import {COMMENT_PARENT_TYPE} from "@/components/comments/_shared";
import CommentSection from "@/components/comments/comment-section";
import IfAuth from "@/components/auth/if-auth";
export default {
  name: "submission",
  components: {IfAuth, CommentSection, VideoPlayer, UserHeader},

  // Defining that submission needs to be passed as an object
  props: {
    submission: {
      type: Object,
      required: true,
    }
  },

  data: () => ({
    showComments: false
  }),

  computed: {
    submissionParentType() {
      return COMMENT_PARENT_TYPE.SUBMISSION
    }
  },

  methods: {
    vote(value){
      if (this.submission.vote === value) return;

      // If it's the 1st time we are voting, add the value (+1), otherwise +2
      this.submission.score += this.submission.vote === 0 ? value : value * 2

      this.submission.vote = value

      return this.$axios.$put(`/api/submissions/${this.submission.id}/vote?value=${value}`, null)
    }
  }

}
</script>

<style scoped>

</style>
