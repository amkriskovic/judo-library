<template>
  <v-card class="my-3">
    <!-- Injecting user-header component, providing username from submission -->
    <user-header :username="submission.user.username" :image-url="submission.user.image" :append="submission.created"/>

    <v-card-text>{{ submission.description }}</v-card-text>

    <!-- Injecting video player component with dynamic binding of video where video is string -->
    <!-- * Getting specific videos from submissions store |> state => fetchSubmissionsForTechnique filling state -->
    <video-player :video="submission.video" :thumb="submission.thumb"/>

    <v-card-actions>
      <span>{{submission.upVotes}}</span>
      <v-btn icon>
        <v-icon>mdi-thumb-up</v-icon>
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
  }
}
</script>

<style scoped>

</style>
