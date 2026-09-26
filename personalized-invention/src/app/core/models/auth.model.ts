export interface RegisterRequest {
  fullName: string;
  email: string;
  password: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface AuthResponse {
  userId: number;
  fullName: string;
  email: string;
<<<<<<< HEAD
=======
  isAdmin: boolean;      // ← new
>>>>>>> beta
  token: string;
  expiresAt: string;
}

export interface CurrentUser {
  userId: number;
  fullName: string;
  email: string;
<<<<<<< HEAD
=======
  isAdmin: boolean;      // ← new
>>>>>>> beta
}