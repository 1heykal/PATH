import { User } from '../models/User';

export interface LoginResponse {
  accessToken: string;
  user: User;
}
