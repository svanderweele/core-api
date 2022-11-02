export type RegisterRequest = {
  name: string;
  email: string;
  password: string;
};

export type RegisterResponse = {
  id: string;
  name: string;
  email: string;
  password: string;
};

export type LoginRequest = {
  email: string;
  password: string;
};

export type LoginResponse = {
  token: string;
};

export type User = {
  email: string;
  name: string;
};
