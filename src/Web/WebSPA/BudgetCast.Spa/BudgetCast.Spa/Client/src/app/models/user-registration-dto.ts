import { UserProfileDto } from '../modules/user-account/models/user-profile-dto';
export class UserRegistrationDto extends UserProfileDto {
  email: string;
  password: string;
  passwordConfirm: string;
}
