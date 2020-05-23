import { UserProfile } from '../modules/user-account/models/user-profile';
export class UserRegistration extends UserProfile {
  email: string;
  password: string;
  passwordConfirm: string;
}
