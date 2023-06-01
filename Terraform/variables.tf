variable "location" {
  default = "westeurope"
}

variable "prefix" {
  default = "mlm"
}

variable "image" {
  default     = "tile38/tile38"
}

variable "port" {
  default     = 9851
}

variable "cpu_cores" {
  default     = 1
}

variable "memory_in_gb" {
  default     = 2
}