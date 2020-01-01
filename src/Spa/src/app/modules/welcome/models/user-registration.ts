import { UserProfile } from './user-profile';
export class UserRegistration extends UserProfile {
  email: string;
  password: string;
  passwordConfirm: string;
}
